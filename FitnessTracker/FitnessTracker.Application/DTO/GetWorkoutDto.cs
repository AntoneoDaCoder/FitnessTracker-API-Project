using FitnessTracker.Core.Abstractions;
namespace FitnessTracker.Application.DTO
{
    public record GetWorkoutDto : IGetWorkoutData
    {
        public string? UserName { get; set; } = string.Empty;
        public string? WorkoutId { get; set; } = string.Empty;
    }
}

