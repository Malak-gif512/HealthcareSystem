namespace HealthcareSystem.Application.DTOs.Patients
{
    // Safe response structure exposing patient data without sensitive backend fields
    public class PatientResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string BloodType { get; set; } = string.Empty;
        public int Age => DateTime.Today.Year - DateOfBirth.Year; // Calculated field
    }
}