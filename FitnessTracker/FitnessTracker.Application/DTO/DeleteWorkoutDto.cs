

using FitnessTracker.Core.Abstractions;

namespace FitnessTracker.Application.DTO
{
    public class DeleteWorkoutDto : IDeleteWorkoutData
    {
        public string? UserName { get; set; } = string.Empty;
        public List<string>? WorkoutIds { get; set; }
    }
}
