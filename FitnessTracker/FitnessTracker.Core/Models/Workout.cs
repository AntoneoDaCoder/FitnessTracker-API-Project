using System.Text.Json.Serialization;

namespace FitnessTracker.Core.Models
{
    public class Workout
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateOnly Date { get; set; } = DateOnly.MinValue;
        public TimeOnly Duration { get; set; }
        public string Sport { get; set; } = string.Empty;
        public double TotalCalories { get; set; }
        [JsonIgnore]
        public User Parent { get; set; }
    }
}
