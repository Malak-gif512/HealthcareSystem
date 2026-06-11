using HealthcareSystem.Application.DTOs;
using HealthcareSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareSystem.Api.Controllers
{
    // API Endpoints for user identity and token generation
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var responseData = await _authService.RegisterAsync(request);
            // Wrapping the response in our standardized ApiResponse format
            return Ok(ApiResponse<AuthResponse>.Ok(responseData, "Registration successful"));

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var responseData = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.Ok(responseData, "Login successful"));
        }
    }
}