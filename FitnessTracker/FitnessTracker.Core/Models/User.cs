using Microsoft.AspNetCore.Identity;
namespace FitnessTracker.Core.Models
{
    public class User : IdentityUser
    {
        public uint Age { get; set; }
        public double Weight { get; set; }
        public uint Height { get; set; }
        public string Gender { get; set; }
        public ICollection<Workout>? Workouts { get; set; }
    }
}
