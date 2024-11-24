using Microsoft.EntityFrameworkCore;
using FitnessTracker.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FitnessTracker.DataAccess.Configurations;

namespace FitnessTracker.DataAccess.Contexts
{
    public class FitnessTrackerDbContext : IdentityDbContext<User>
    {
        public DbSet<Workout> Workouts { get; set; }
        public FitnessTrackerDbContext(DbContextOptions<FitnessTrackerDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConf());
            builder.ApplyConfiguration(new WorkoutConf());
        }
    }
}
