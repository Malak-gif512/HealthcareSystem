namespace HealthcareSystem.Domain.Entities
{
    // Base class providing common auditing and soft-delete properties for all entities
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false; // Enables Soft Delete
    }
}