using HealthcareSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareSystem.Infrastructure.Persistence.Configurations
{
    // Configuration and constraints for AuditLog entity
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            // Restricting column sizes for performance
            builder.Property(a => a.EntityName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Action).IsRequired().HasMaxLength(50);
            builder.Property(a => a.ChangedBy).IsRequired().HasMaxLength(150);
            // Changes property will store JSON strings, allowing default max length
        }
    }
}