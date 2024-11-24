using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using FitnessTracker.Core.Abstractions;
using System.Text;
namespace FitnessTracker.Application.Services
{
    public class RabbitWorker : IWorker
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection? _connection;
        private IChannel? _channel;
        public RabbitWorker()
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("REMOTE_HOST"),
                UserName = Environment.GetEnvironmentVariable("REMOTE_USER"),
                Password = Environment.GetEnvironmentVariable("REMOTE_PASS"),
                Port = int.Parse(Environment.GetEnvironmentVariable("REMOTE_PORT"))
            };
        }
        public async Task InitServiceAsync(List<string> queues)
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            foreach (var wQueue in queues)
                await _channel.QueueDeclareAsync(queue: wQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
        }
        public async Task StartListeningAsync(string who, Func<string, Task<string>> handler)
        {
            if (_channel == null)
            {
                throw new InvalidOperationException();
            }
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (object sender, BasicDeliverEventArgs ea) =>
            {
                AsyncEventingBasicConsumer cons = (AsyncEventingBasicConsumer)sender;
                IChannel ch = cons.Channel;
                string response = string.Empty;

                byte[] body = ea.Body.ToArray();
                IReadOnlyBasicProperties props = ea.BasicProperties;
                var replyProps = new BasicProperties
                {
                    CorrelationId = props.CorrelationId
                };

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    response = await handler(message);
                }
                catch (Exception e)
                {
                    response = string.Empty;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await ch.BasicPublishAsync(exchange: string.Empty, routingKey: props.ReplyTo!,
                        mandatory: true, basicProperties: replyProps, body: responseBytes);
                    await ch.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            await _channel.BasicConsumeAsync(who, false, consumer);
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
