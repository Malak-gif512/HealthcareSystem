using HealthcareSystem.Application.DTOs;
using HealthcareSystem.Application.DTOs.Patients;
using HealthcareSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareSystem.Api.Controllers
{
    // Secure API endpoints for clinical data engine
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Enforces that ALL endpoints require a valid JWT token globally
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Nurse")] // Role-Based Access Control (RBAC) applied
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request)
        {
            var responseData = await _patientService.CreatePatientAsync(request);
            // Returns 201 Created with the location of the new resource
            return CreatedAtAction(nameof(GetPatientById), new { id = responseData.Id },
                ApiResponse<PatientResponse>.Ok(responseData, "Patient profile created successfully."));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Nurse")] // Patients shouldn't see all other patients
        public async Task<IActionResult> GetPatients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            var responseData = await _patientService.GetPatientsAsync(pageNumber, pageSize, searchTerm);
            return Ok(ApiResponse<PagedResult<PatientResponse>>.Ok(responseData, "Patients retrieved successfully."));
        }

        [HttpGet("{id}")]
        // Any authenticated user can access this, but in a real-world scenario, 
        // we'd add logic to ensure a patient only fetches their own ID.
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            var responseData = await _patientService.GetPatientByIdAsync(id);
            return Ok(ApiResponse<PatientResponse>.Ok(responseData, "Patient retrieved successfully."));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Nurse")]
        public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] CreatePatientRequest request)
        {
            var responseData = await _patientService.UpdatePatientAsync(id, request);
            return Ok(ApiResponse<PatientResponse>.Ok(responseData, "Patient profile updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admins should be allowed to delete records
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            var success = await _patientService.DeletePatientAsync(id);
            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Patient profile not found."));

            return Ok(ApiResponse<bool>.Ok(true, "Patient profile deleted successfully."));
        }
    }
}