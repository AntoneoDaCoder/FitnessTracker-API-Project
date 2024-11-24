using FitnessTracker.Application.DTO;
using FitnessTracker.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
namespace FitnessTracker.Application.Services
{
    public class StatService : IWorkoutService
    {
        private readonly IWorker _worker;
        private readonly IServiceScopeFactory _scopeFactory;
        public StatService(IWorker worker, IServiceScopeFactory scopeFactory)
        {
            _worker = worker;
            _scopeFactory = scopeFactory;
        }
        public async void StartListeningAsync()
        {
            await _worker.InitServiceAsync(new() { "workouts-stat" });
            await _worker.StartListeningAsync("workouts-stat", GetUserStatAsync);
        }
        private async Task<string> GetUserStatAsync(string data)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repos = scope.ServiceProvider.GetRequiredService<IWorkoutsRepository>();
                try
                {
                   GetStatsDto statsDto = JsonSerializer.Deserialize<GetStatsDto>(data);
                    string stats = await repos.GetUserStatsAsync(statsDto);
                    return stats;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
