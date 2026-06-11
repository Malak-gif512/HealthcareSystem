using HealthcareSystem.Application.DTOs;
using HealthcareSystem.Application.Interfaces;
using HealthcareSystem.Domain.Entities;

namespace HealthcareSystem.Application.Services
{
    // Orchestrates authentication logic securely
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        // Dependency Injection
        public AuthService(
            IGenericRepository<User> userRepository,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // 1. Check if email already exists
            var existingUser = await _userRepository.FindAsync(u => u.Email == request.Email);
            if (existingUser != null)
                throw new Exception("Email is already registered."); // In production, use custom exceptions

            // 2. Create the user entity and hash the password
            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                Role = request.Role
            };

            // 3. Save to database to generate the User ID
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            // 4. Generate Tokens
            var token = _jwtProvider.GenerateToken(newUser);
            var refreshToken = _jwtProvider.GenerateRefreshToken();

            // 5. Save Refresh Token in Database for future validation
            newUser.RefreshToken = refreshToken;
            newUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Valid for 7 days

            _userRepository.Update(newUser);
            await _userRepository.SaveChangesAsync();

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                Email = newUser.Email,
                Role = newUser.Role.ToString()
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // 1. Find user by email
            var user = await _userRepository.FindAsync(u => u.Email == request.Email);
            if (user == null)
                throw new Exception("Invalid email or password.");

            // 2. Verify password
            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new Exception("Invalid email or password.");

            // 3. Generate new JWT Token
            var token = _jwtProvider.GenerateToken(user);
            var refreshToken = _jwtProvider.GenerateRefreshToken();

            // 4. Save Refresh Token in DB
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Valid for 7 days

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}