namespace HealthcareSystem.Domain.Entities
{
    // Represents a standardized medical history or clinical note for a patient
    public class ClinicalRecord : BaseEntity
    {
        public Guid PatientProfileId { get; set; } // Foreign key
        public string Diagnosis { get; set; } = string.Empty;
        public string ClinicalNotes { get; set; } = string.Empty;

        // Navigation property
        public PatientProfile PatientProfile { get; set; } = null!;
    }
}