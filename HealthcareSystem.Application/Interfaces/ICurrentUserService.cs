namespace HealthcareSystem.Application.Interfaces
{
    // Contract to retrieve the currently authenticated user's ID
    public interface ICurrentUserService
    {
        string? UserId { get; }
    }
}