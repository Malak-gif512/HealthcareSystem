using System.ComponentModel.DataAnnotations;

namespace HealthcareSystem.Application.DTOs.Patients
{
    // Payload for creating a new patient profile linked to a user account
    public class CreatePatientRequest
    {
        [Required]
        public Guid UserId { get; set; } // Must be an existing User with Role = Patient

        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(5)]
        public string BloodType { get; set; } = string.Empty;
    }
}