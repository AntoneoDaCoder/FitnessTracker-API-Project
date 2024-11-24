
namespace FitnessTracker.Core.Abstractions
{
    public interface IGetStats
    {
        string? UserName { get; }
        DateOnly From { get; }
        DateOnly To { get; }
    }
}
