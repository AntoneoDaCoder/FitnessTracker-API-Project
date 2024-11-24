using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessTracker.Core.Models;

namespace FitnessTracker.DataAccess.Configurations
{
    public class WorkoutConf : IEntityTypeConfiguration<Workout>
    {
        public void Configure(EntityTypeBuilder<Workout> builder)
        {
            builder.HasKey(w => w.Id);

            builder.HasOne(w => w.Parent)
                .WithMany(usr => usr.Workouts)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade).IsRequired();

            builder.Property(w => w.Id)
                .IsRequired();
            builder.Property(w => w.Date)
                .IsRequired();
            builder.Property(w => w.Duration)
                .IsRequired();
            builder.Property(w => w.Sport)
                .IsRequired();
            builder.Property(w => w.TotalCalories)
                .IsRequired();
            builder.Property(w => w.UserId)
              .IsRequired();
            builder.HasIndex(w => new { w.Sport, w.Date });
        }
    }
}
