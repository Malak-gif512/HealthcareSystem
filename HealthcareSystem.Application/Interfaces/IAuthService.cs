using HealthcareSystem.Application.DTOs;

namespace HealthcareSystem.Application.Interfaces
{
    // Contract for authentication and identity operations
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}