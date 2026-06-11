using HealthcareSystem.Application.DTOs;
using HealthcareSystem.Application.DTOs.Appointments;
using HealthcareSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Globally secured endpoint
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Patient")] // Nurses don't book, patients book for themselves
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
        {
            var responseData = await _appointmentService.CreateAppointmentAsync(request);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = responseData.Id },
                ApiResponse<AppointmentResponse>.Ok(responseData, "Appointment scheduled successfully."));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Nurse")] // Staff can view all appointments, patients should ideally have a separate endpoint for "My Appointments"
        public async Task<IActionResult> GetAppointments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? locationArea = null)
        {
            var responseData = await _appointmentService.GetAppointmentsAsync(pageNumber, pageSize, locationArea);
            return Ok(ApiResponse<PagedResult<AppointmentResponse>>.Ok(responseData, "Appointments retrieved successfully."));
        }

        [HttpGet("my-appointments")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyAppointments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Extract the User ID securely from the JWT Token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            var responseData = await _appointmentService.GetPatientAppointmentsAsync(userId, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<AppointmentResponse>>.Ok(responseData, "Your appointments retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(Guid id)
        {
            var responseData = await _appointmentService.GetAppointmentByIdAsync(id);
            return Ok(ApiResponse<AppointmentResponse>.Ok(responseData, "Appointment retrieved successfully."));
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Nurse")] // Only staff can confirm, complete, or officially cancel
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateAppointmentStatusRequest request)
        {
            var responseData = await _appointmentService.UpdateAppointmentStatusAsync(id, request);
            return Ok(ApiResponse<AppointmentResponse>.Ok(responseData, "Appointment status updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            var success = await _appointmentService.DeleteAppointmentAsync(id);
            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Appointment not found."));

            return Ok(ApiResponse<bool>.Ok(true, "Appointment canceled and removed successfully."));
        }
    }
}