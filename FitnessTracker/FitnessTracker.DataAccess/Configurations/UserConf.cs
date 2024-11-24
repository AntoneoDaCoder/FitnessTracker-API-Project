using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessTracker.Core.Models;
namespace FitnessTracker.DataAccess.Configurations
{
    public class UserConf : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(u => u.UserName).IsUnique();

            builder.Property(u => u.Weight)
                .IsRequired();
            builder.Property(u => u.Height)
                .IsRequired();
            builder.Property(u => u.Age)
                .IsRequired();
            builder.Property(u => u.Gender)
                .IsRequired();
            builder.HasIndex(u => new { u.Weight, u.Age, u.Gender });
        }
    }
}
