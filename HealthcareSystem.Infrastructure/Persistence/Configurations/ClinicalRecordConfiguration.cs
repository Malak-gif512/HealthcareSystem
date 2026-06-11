using HealthcareSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareSystem.Infrastructure.Persistence.Configurations
{
    // Configuration and constraints for ClinicalRecord entity
    public class ClinicalRecordConfiguration : IEntityTypeConfiguration<ClinicalRecord>
    {
        public void Configure(EntityTypeBuilder<ClinicalRecord> builder)
        {
            // Global Query Filter for Soft Delete
            builder.HasQueryFilter(c => !c.IsDeleted);

            // Defensive payload constraints
            builder.Property(c => c.Diagnosis).IsRequired().HasMaxLength(500);
            // ClinicalNotes might be long, so we don't set a max length (defaults to nvarchar(max))
            builder.Property(c => c.ClinicalNotes).IsRequired();
        }
    }
}