using HealthcareSystem.Domain.Enums;

namespace HealthcareSystem.Application.DTOs.Appointments
{
    public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public Guid PatientProfileId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public AppointmentStatus Status { get; set; }
        public string LocationArea { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}