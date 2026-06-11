using HealthcareSystem.Domain.Entities;

namespace HealthcareSystem.Application.Interfaces
{
    // Contract for generating standard JWT access tokens
    public interface IJwtProvider
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
    }
}