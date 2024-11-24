using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using FitnessTracker.Core.Abstractions;
using System.Collections.Concurrent;
using System.Text;
namespace FitnessTracker.Application.Services
{
    public class RabbitClient : IClient
    {
        private readonly IConnectionFactory _connectionFactory;
        private ConcurrentDictionary<string, TaskCompletionSource<string>> _callbackMapper = new();
        private ConcurrentDictionary<string, string> _callbackQueues = new();
        private IConnection? _connection;
        private IChannel? _channel;
        public RabbitClient()
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("REMOTE_HOST"),
                UserName = Environment.GetEnvironmentVariable("REMOTE_USER"),
                Password = Environment.GetEnvironmentVariable("REMOTE_PASS"),
                Port = int.Parse(Environment.GetEnvironmentVariable("REMOTE_PORT"))
            };
        }
        public async Task InitServiceAsync()
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }
        public async void InitCallbackQueue(string whose)
        {
            if (_channel == null)
            {
                throw new InvalidOperationException();
            }
            //временная очередь под ответ
            QueueDeclareOk declareResult = await _channel.QueueDeclareAsync(queue: "", durable: false, exclusive: true, autoDelete: true, arguments: null);
            var responseQueue = declareResult.QueueName;
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += (model, ea) =>
            {
                string? corrId = ea.BasicProperties.CorrelationId;
                if (!string.IsNullOrEmpty(corrId))
                {
                    //если есть задача, для которой ожидается ответ от рабочего
                    //то убираем ее из контейнера задач и завершаем ее с ответом от рабочего
                    if (_callbackMapper.TryRemove(corrId, out var callback))
                    {
                        var msgBytes = ea.Body.ToArray();
                        var response = Encoding.UTF8.GetString(msgBytes);
                        callback.TrySetResult(response);
                    }
                }
                return Task.CompletedTask;
            };
            await _channel.BasicConsumeAsync(responseQueue, true, consumer);
            _callbackQueues.TryAdd(whose, responseQueue);
        }
        public async Task<string> CallAsync(string who, string msg, CancellationToken cancellationToken)
        {
            if (_channel == null)
            {
                throw new InvalidOperationException();
            }

            if (!_callbackQueues.TryGetValue(who, out var callback))
            {
                throw new InvalidOperationException();
            }
            //сам запрос к микросервису через брокер
            string corrId = Guid.NewGuid().ToString();
            var props = new BasicProperties()
            {
                CorrelationId = corrId,
                ReplyTo = callback,
            };

            //создаем задачу, которая будет ждать ответ от удаленного рабочего либо умрет нахуй если вышло время
            //(очередь при этом останется жива, т.е сохранится все необработанные сообщения)
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _callbackMapper.TryAdd(corrId, tcs);

            var msgBytes = Encoding.UTF8.GetBytes(msg);
            await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: who, mandatory: true, basicProperties: props, body: msgBytes);

            //указываем че делать если произошел таймаут (передан в метод, задается в контроллере)
            using CancellationTokenRegistration ctr = cancellationToken.Register(() =>
            {
                _callbackMapper.TryRemove(corrId, out _);
                tcs.SetException(new TimeoutException());
            });

            return await tcs.Task;
        }
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(disposing: true);
            GC.SuppressFinalize(this);
        }
        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                if (_channel is not null)
                    await _channel.CloseAsync();
                if (_connection is not null)
                    await _connection.CloseAsync();
            }
        }
    }
}
