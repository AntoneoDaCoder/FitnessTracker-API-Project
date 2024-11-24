using FitnessTracker.Core.Abstractions;

namespace FitnessTracker.Application.DTO
{
    public class GetStatsDto : IGetStats
    {
        public string? UserName { get; set; } = string.Empty;
        public DateOnly From { get; set; } = DateOnly.MinValue;
        public DateOnly To { get; set; } = DateOnly.MaxValue;
    }
}
