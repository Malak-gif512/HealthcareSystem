namespace HealthcareSystem.Domain.Entities
{
    // Contains pure clinical and medical information, separated from authentication
    public class PatientProfile : BaseEntity
    {
        public Guid UserId { get; set; } // Foreign key to User table
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string BloodType { get; set; } = string.Empty;

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<ClinicalRecord> ClinicalRecords { get; set; } = new List<ClinicalRecord>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}