using HealthcareSystem.Application.DTOs;
using HealthcareSystem.Application.DTOs.Patients;

namespace HealthcareSystem.Application.Interfaces
{
    // Service contract for managing clinical patient profiles
    public interface IPatientService
    {
        Task<PatientResponse> CreatePatientAsync(CreatePatientRequest request);
        Task<PagedResult<PatientResponse>> GetPatientsAsync(int pageNumber, int pageSize, string? searchTerm);
        Task<PatientResponse> GetPatientByIdAsync(Guid id);
        Task<PatientResponse> UpdatePatientAsync(Guid id, CreatePatientRequest request);
        Task<bool> DeletePatientAsync(Guid id);
    }
}