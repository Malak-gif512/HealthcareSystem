namespace HealthcareSystem.Domain.Entities
{
    // Immutable transactional log tracking critical data alterations
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EntityName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // e.g., Insert, Update, Delete
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Changes { get; set; } = string.Empty; // JSON representation of changes
        public string ChangedBy { get; set; } = string.Empty; // User ID
    }
}