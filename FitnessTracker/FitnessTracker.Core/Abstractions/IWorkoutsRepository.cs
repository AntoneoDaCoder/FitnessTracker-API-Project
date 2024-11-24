using FitnessTracker.Core.Models;
namespace FitnessTracker.Core.Abstractions;
//rewrite it
public interface IWorkoutsRepository
{
    Task<string> CreateUserWorkoutAsync(IAddWorkoutData validWorkoutDto);
    Task<string> DeleteUserWorkoutAsync(IDeleteWorkoutData validWorkoutDto);
    Task<string> GetUserWorkoutsAsync(IGetWorkoutData validWorkoutDto);
    Task<string> UpdateUserWorkoutAsync(IEditWorkoutData validWorkoutDto);
    Task<string> GetUserStatsAsync(IGetStats validStatsDto);
}