using System.Text.Json;
using HealthcareSystem.Application.Interfaces;
using HealthcareSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthcareSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        // Injecting ICurrentUserService to know WHO is making the changes
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PatientProfile> PatientProfiles { get; set; }
        public DbSet<ClinicalRecord> ClinicalRecords { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        // Intercepts save operations to apply soft-delete AND global Audit Logging
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = new List<AuditLog>();
            var userId = _currentUserService.UserId ?? "System"; // Fallback for system-level changes

            // ChangeTracker.Entries<BaseEntity>() automatically EXCLUDES AuditLog 
            // because AuditLog doesn't inherit from BaseEntity. This is perfect!
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                // Skip entities that are not being modified
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var action = entry.State.ToString();
                var entityName = entry.Entity.GetType().Name;
                var changes = new Dictionary<string, object>();

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        // Record all current values for new entities
                        foreach (var prop in entry.Properties)
                            changes[prop.Metadata.Name] = prop.CurrentValue!;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        // Record only properties that actually changed
                        foreach (var prop in entry.Properties.Where(p => p.IsModified))
                            changes[prop.Metadata.Name] = new { Old = prop.OriginalValue, New = prop.CurrentValue };
                        break;

                    case EntityState.Deleted:
                        // Soft delete logic overrides hard delete
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        action = "SoftDeleted";
                        changes["IsDeleted"] = new { Old = false, New = true };
                        break;
                }

                // Create the immutable audit log entry
                auditEntries.Add(new AuditLog
                {
                    EntityName = entityName,
                    Action = action,
                    ChangedBy = userId,
                    Changes = JsonSerializer.Serialize(changes),
                    Timestamp = DateTime.UtcNow // Corrected to use Timestamp as defined in AuditLog.cs
                });
            }

            // Insert audit logs into the tracker before saving
            if (auditEntries.Any())
            {
                await AuditLogs.AddRangeAsync(auditEntries, cancellationToken);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}