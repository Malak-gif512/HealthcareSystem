using HealthcareSystem.Domain.Enums;

namespace HealthcareSystem.Domain.Entities
{
    // Represents the identity and authentication details for any person logging in
    public class User : BaseEntity
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }

        // Navigation property: A user MIGHT have a patient profile if their role is Patient
        public PatientProfile? PatientProfile { get; set; }
    }
}
