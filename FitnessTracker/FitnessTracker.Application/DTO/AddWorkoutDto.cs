using FitnessTracker.Core.Abstractions;
namespace FitnessTracker.Application.DTO
{
    public record AddWorkoutDto : IAddWorkoutData
    {
        public string? UserName { get; set; } = string.Empty;
        public int AveragePulse { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Duration { get; set; }
        public string? Sport { get; set; } = string.Empty;

    }
}
