namespace HealthcareSystem.Application.DTOs
{
    // Standardized response containing the JWT token
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}