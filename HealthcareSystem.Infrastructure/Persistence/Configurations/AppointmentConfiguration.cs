using HealthcareSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareSystem.Infrastructure.Persistence.Configurations
{
    // Configuration and constraints for Appointment entity
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            // Global Query Filter for Soft Delete
            builder.HasQueryFilter(a => !a.IsDeleted);

            // Defensive payload constraints
            builder.Property(a => a.LocationArea).IsRequired().HasMaxLength(150);
            builder.Property(a => a.Notes).HasMaxLength(1000);
        }
    }
}