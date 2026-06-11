using System.ComponentModel.DataAnnotations;
using HealthcareSystem.Domain.Enums;

namespace HealthcareSystem.Application.DTOs
{
    // Payload for user registration with basic validation
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
             ErrorMessage = "Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }
    }
}