using System.ComponentModel.DataAnnotations;

namespace HealthcareSystem.Application.DTOs.ClinicalRecords
{
    // Payload for creating a new clinical note or diagnosis for a patient
    public class CreateClinicalRecordRequest
    {
        [Required]
        public Guid PatientProfileId { get; set; }

        [Required, MaxLength(500)]
        public string Diagnosis { get; set; } = string.Empty;

        [Required]
        public string ClinicalNotes { get; set; } = string.Empty;
    }
}