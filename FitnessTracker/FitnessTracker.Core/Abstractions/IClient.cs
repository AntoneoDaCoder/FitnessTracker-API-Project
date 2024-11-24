namespace FitnessTracker.Core.Abstractions;
public interface IClient : IAsyncDisposable
{
    Task<string> CallAsync(string who, string msg, CancellationToken cancellationToken);
    void InitCallbackQueue(string whose);
    Task InitServiceAsync();
}