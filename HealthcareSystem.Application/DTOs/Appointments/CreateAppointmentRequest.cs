using System.ComponentModel.DataAnnotations;

namespace HealthcareSystem.Application.DTOs.Appointments
{
    // Payload for creating a new booking with geographical constraints
    public class CreateAppointmentRequest
    {
        [Required]
        public Guid PatientProfileId { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required, MaxLength(150)]
        public string LocationArea { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;
    }
}