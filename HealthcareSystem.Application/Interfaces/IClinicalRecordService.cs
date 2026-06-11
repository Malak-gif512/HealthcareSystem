using HealthcareSystem.Application.DTOs;
using HealthcareSystem.Application.DTOs.ClinicalRecords;

namespace HealthcareSystem.Application.Interfaces
{
    // Service contract for managing medical histories and clinical notes
    public interface IClinicalRecordService
    {
        Task<ClinicalRecordResponse> CreateRecordAsync(CreateClinicalRecordRequest request);

        // Updated to return a PagedResult
        Task<PagedResult<ClinicalRecordResponse>> GetRecordsByPatientIdAsync(Guid patientProfileId, int pageNumber, int pageSize);

        Task<ClinicalRecordResponse> GetRecordByIdAsync(Guid id);

        // Added Update and Delete
        Task<ClinicalRecordResponse> UpdateRecordAsync(Guid id, CreateClinicalRecordRequest request);
        Task<bool> DeleteRecordAsync(Guid id);
    }
}