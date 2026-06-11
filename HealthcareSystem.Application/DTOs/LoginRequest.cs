using System.ComponentModel.DataAnnotations;

namespace HealthcareSystem.Application.DTOs
{
    // Payload for authentication
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}