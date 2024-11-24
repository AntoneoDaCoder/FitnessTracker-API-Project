namespace FitnessTracker.Core.Abstractions
{
    public interface IGetWorkoutData
    {
        string? UserName{ get; }
        string? WorkoutId { get; }
    }
}
