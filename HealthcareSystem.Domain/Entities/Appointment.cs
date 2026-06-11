using HealthcareSystem.Domain.Enums;

namespace HealthcareSystem.Domain.Entities
{
    // Manages real-time clinical allocations and status tracking
    public class Appointment : BaseEntity
    {
        public Guid PatientProfileId { get; set; } // Foreign key
        public DateTime ScheduledDate { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string Notes { get; set; } = string.Empty;

        // Fulfilling the geographical constraints requirement
        public string LocationArea { get; set; } = string.Empty;

        // Navigation property
        public PatientProfile PatientProfile { get; set; } = null!;
    }
}