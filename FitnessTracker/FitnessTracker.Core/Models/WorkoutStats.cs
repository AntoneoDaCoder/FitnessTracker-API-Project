using FitnessTracker.Core.Abstractions;
namespace FitnessTracker.Core.Models
{
    public class WorkoutStats
    {
        public double AverageCalories { get; set; }
        public double MaxCalories { get; set; }
        public double MinCalories { get; set; }
        public TimeOnly AverageDuration { get; set; }
        public TimeOnly MaxDuration { get; set; }
        public TimeOnly MinDuration { get; set; }
        public ICollection<Workout> Workouts { get; set; }
    }
}
