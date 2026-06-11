using HealthcareSystem.Application.DTOs;
using HealthcareSystem.Application.DTOs.ClinicalRecords;
using HealthcareSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareSystem.Api.Controllers
{
    // Secure API endpoints for managing medical histories
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires JWT Token
    public class ClinicalRecordsController : ControllerBase
    {
        private readonly IClinicalRecordService _recordService;

        public ClinicalRecordsController(IClinicalRecordService recordService)
        {
            _recordService = recordService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Nurse")] // Only medical staff can add records
        public async Task<IActionResult> CreateRecord([FromBody] CreateClinicalRecordRequest request)
        {
            var responseData = await _recordService.CreateRecordAsync(request);
            return CreatedAtAction(nameof(GetRecordById), new { id = responseData.Id },
                ApiResponse<ClinicalRecordResponse>.Ok(responseData, "Clinical record added successfully."));
        }

        [HttpGet("patient/{patientProfileId}")]
        [Authorize(Roles = "Admin,Nurse,Patient")]
        public async Task<IActionResult> GetPatientRecords(Guid patientProfileId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var responseData = await _recordService.GetRecordsByPatientIdAsync(patientProfileId, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<ClinicalRecordResponse>>.Ok(responseData, "Medical history retrieved successfully."));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Nurse,Patient")]
        public async Task<IActionResult> GetRecordById(Guid id)
        {
            var responseData = await _recordService.GetRecordByIdAsync(id);
            return Ok(ApiResponse<ClinicalRecordResponse>.Ok(responseData, "Record retrieved successfully."));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Nurse")] // Patients cannot edit clinical notes
        public async Task<IActionResult> UpdateRecord(Guid id, [FromBody] CreateClinicalRecordRequest request)
        {
            var responseData = await _recordService.UpdateRecordAsync(id, request);
            return Ok(ApiResponse<ClinicalRecordResponse>.Ok(responseData, "Clinical record updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Strict RBAC: Only Admins can soft-delete clinical records
        public async Task<IActionResult> DeleteRecord(Guid id)
        {
            var success = await _recordService.DeleteRecordAsync(id);
            if (!success)
                return NotFound(ApiResponse<bool>.Fail("Clinical record not found."));

            return Ok(ApiResponse<bool>.Ok(true, "Clinical record deleted successfully."));
        }
    }
}