namespace HealthcareSystem.Application.DTOs.ClinicalRecords
{
    // Safe response structure for clinical records
    public class ClinicalRecordResponse
    {
        public Guid Id { get; set; }
        public Guid PatientProfileId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string ClinicalNotes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}