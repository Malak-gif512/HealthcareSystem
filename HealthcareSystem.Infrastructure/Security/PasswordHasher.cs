using HealthcareSystem.Application.Interfaces;

namespace HealthcareSystem.Infrastructure.Security
{
    // Implementation of password hashing using the industry-standard BCrypt algorithm
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Work factor of 12 is a secure standard balancing performance and security
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}