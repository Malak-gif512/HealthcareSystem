using HealthcareSystem.Application.DTOs;
using HealthcareSystem.Application.DTOs.Appointments;

namespace HealthcareSystem.Application.Interfaces
{
    // Contract for the transactional booking engine
    public interface IAppointmentService
    {
        Task<AppointmentResponse> CreateAppointmentAsync(CreateAppointmentRequest request);
        Task<PagedResult<AppointmentResponse>> GetAppointmentsAsync(int pageNumber, int pageSize, string? locationArea);
        Task<PagedResult<AppointmentResponse>> GetPatientAppointmentsAsync(Guid userId, int pageNumber, int pageSize);
        Task<AppointmentResponse> GetAppointmentByIdAsync(Guid id);
        Task<AppointmentResponse> UpdateAppointmentStatusAsync(Guid id, UpdateAppointmentStatusRequest request);
        Task<bool> DeleteAppointmentAsync(Guid id);
    }
}