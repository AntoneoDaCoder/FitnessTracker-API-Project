namespace FitnessTracker.Core.Abstractions
{
    public interface IAddWorkoutData
    {
        string? UserName { get; }
        int AveragePulse { get; }
        DateOnly Date { get; }
        TimeOnly Duration { get; }
        string? Sport { get; }
    }
}
