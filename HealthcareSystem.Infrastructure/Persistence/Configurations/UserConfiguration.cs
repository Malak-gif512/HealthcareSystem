using HealthcareSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareSystem.Infrastructure.Persistence.Configurations
{
    // Separated configuration for User entity
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Global Query Filter for Soft Delete
            builder.HasQueryFilter(u => !u.IsDeleted);

            // Defensive payload constraints
            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
            builder.HasIndex(u => u.Email).IsUnique(); // Ensure no duplicate emails

            // Configuring the One-to-One relationship
            builder.HasOne(u => u.PatientProfile)
                .WithOne(p => p.User)
                .HasForeignKey<PatientProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);// Prevent cascading physical deletes
        }
    }
}