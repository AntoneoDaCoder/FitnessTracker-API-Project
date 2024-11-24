using FitnessTracker.Application.DTO;
using FitnessTracker.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
namespace FitnessTracker.Application.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly IWorker _worker;
        private readonly IServiceScopeFactory _scopeFactory;
        public WorkoutService(IWorker worker, IServiceScopeFactory scopeFactory)
        {
            _worker = worker;
            _scopeFactory = scopeFactory;
        }
        public async void StartListeningAsync()
        {
            await _worker.InitServiceAsync(new() { "workouts-add", "workouts-update", "workouts-get", "workouts-delete" });
            await _worker.StartListeningAsync("workouts-add", AddWorkoutAsync);
            await _worker.StartListeningAsync("workouts-update", UpdateWorkoutAsync);
            await _worker.StartListeningAsync("workouts-get", GetWorkoutAsync);
            await _worker.StartListeningAsync("workouts-delete", DeleteWorkoutAsync);
        }
        private async Task<string> AddWorkoutAsync(string data)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repos = scope.ServiceProvider.GetRequiredService<IWorkoutsRepository>();
                try
                {
                    AddWorkoutDto newWorkout = JsonSerializer.Deserialize<AddWorkoutDto>(data);
                    string w = await repos.CreateUserWorkoutAsync(newWorkout);
                    return w;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        private async Task<string> UpdateWorkoutAsync(string data)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repos = scope.ServiceProvider.GetRequiredService<IWorkoutsRepository>();
                try
                {
                    EditWorkoutDto newWorkout = JsonSerializer.Deserialize<EditWorkoutDto>(data);
                    string w = await repos.UpdateUserWorkoutAsync(newWorkout);
                    return w;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        private async Task<string> DeleteWorkoutAsync(string data)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repos = scope.ServiceProvider.GetRequiredService<IWorkoutsRepository>();
                try
                {
                    DeleteWorkoutDto targetWorkout = JsonSerializer.Deserialize<DeleteWorkoutDto>(data);
                    string workouts = await repos.DeleteUserWorkoutAsync(targetWorkout);
                    return workouts;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        private async Task<string> GetWorkoutAsync(string data)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repos = scope.ServiceProvider.GetRequiredService<IWorkoutsRepository>();
                try
                {
                    GetWorkoutDto newWorkout = JsonSerializer.Deserialize<GetWorkoutDto>(data);
                    string workouts = await repos.GetUserWorkoutsAsync(newWorkout);
                    return workouts;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
