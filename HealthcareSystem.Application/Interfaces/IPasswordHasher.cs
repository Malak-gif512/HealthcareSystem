namespace HealthcareSystem.Application.Interfaces
{
    // Contract for securely hashing and verifying passwords
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}