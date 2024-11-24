
namespace FitnessTracker.Core.Abstractions;

public interface IWorker : IAsyncDisposable
{
    Task InitServiceAsync(List<string> queues);
    Task StartListeningAsync(string who, Func<string, Task<string>> handler);
}