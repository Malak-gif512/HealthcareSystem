using System.ComponentModel.DataAnnotations;
using HealthcareSystem.Domain.Enums;

namespace HealthcareSystem.Application.DTOs.Appointments
{
    // Payload specifically for chronological status tracking
    public class UpdateAppointmentStatusRequest
    {
        [Required]
        public AppointmentStatus Status { get; set; }
    }
}