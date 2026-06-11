using HealthcareSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareSystem.Infrastructure.Persistence.Configurations
{
    // Configuration and constraints for PatientProfile entity
    public class PatientProfileConfiguration : IEntityTypeConfiguration<PatientProfile>
    {
        public void Configure(EntityTypeBuilder<PatientProfile> builder)
        {
            // Global Query Filter for Soft Delete
            builder.HasQueryFilter(p => !p.IsDeleted);

            // Defensive payload constraints
            builder.Property(p => p.FullName).IsRequired().HasMaxLength(200);
            builder.Property(p => p.BloodType).HasMaxLength(5); // e.g., "AB+", "O-"
        }
    }
}