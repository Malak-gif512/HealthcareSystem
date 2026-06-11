using HealthcareSystem.Application.DTOs;
using HealthcareSystem.Application.DTOs.Appointments;
using HealthcareSystem.Application.Interfaces;
using HealthcareSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthcareSystem.Application.Services
{
    // Orchestrates clinical allocations and guarantees no scheduling conflicts
    public class AppointmentService : IAppointmentService
    {
        private readonly IGenericRepository<Appointment> _appointmentRepository;
        private readonly IGenericRepository<PatientProfile> _patientRepository;

        public AppointmentService(
            IGenericRepository<Appointment> appointmentRepository,
            IGenericRepository<PatientProfile> patientRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
        }

        public async Task<AppointmentResponse> CreateAppointmentAsync(CreateAppointmentRequest request)
        {
            // 1. Validate future date
            if (request.ScheduledDate <= DateTime.UtcNow)
                throw new Exception("Appointments must be scheduled in the future.");

            // 2. Validate patient exists
            var patient = await _patientRepository.GetByIdAsync(request.PatientProfileId);
            if (patient == null)
                throw new Exception("Patient profile not found.");

            // 3. CONFLICT PREVENTION (Transactional Logic): 
            // Assume each appointment takes 30 minutes. Check if this LocationArea is already booked at this time.
            var appointmentEndTime = request.ScheduledDate.AddMinutes(30);

            var hasConflict = await _appointmentRepository.GetQueryable()
                .AnyAsync(a => a.LocationArea == request.LocationArea &&
                               a.Status != Domain.Enums.AppointmentStatus.Canceled &&
                               a.ScheduledDate < appointmentEndTime &&
                               a.ScheduledDate.AddMinutes(30) > request.ScheduledDate);

            if (hasConflict)
                throw new Exception("Scheduling conflict: This location is already booked for the selected time slot.");

            // 4. Create and save
            var appointment = new Appointment
            {
                PatientProfileId = request.PatientProfileId,
                ScheduledDate = request.ScheduledDate,
                LocationArea = request.LocationArea,
                Notes = request.Notes,
                Status = Domain.Enums.AppointmentStatus.Pending // Default status
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return MapToResponse(appointment);
        }

        public async Task<PagedResult<AppointmentResponse>> GetAppointmentsAsync(int pageNumber, int pageSize, string? locationArea)
        {
            var query = _appointmentRepository.GetQueryable();

            // Applying geographical constraints filtering
            if (!string.IsNullOrWhiteSpace(locationArea))
            {
                query = query.Where(a => a.LocationArea == locationArea);
            }

            var totalCount = await query.CountAsync();

            var appointments = await query
                .OrderBy(a => a.ScheduledDate) // Order by closest appointments first
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<AppointmentResponse>
            {
                Items = appointments.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<AppointmentResponse>> GetPatientAppointmentsAsync(Guid userId, int pageNumber, int pageSize)
        {
            // Find appointments where the associated patient profile belongs to this User ID
            var query = _appointmentRepository.GetQueryable()
                //.Include(a => a.PatientProfile) // Need EF Core Include to access the relation
                .Where(a => a.PatientProfile.UserId == userId);

            var totalCount = await query.CountAsync();
            var appointments = await query
                .OrderBy(a => a.ScheduledDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<AppointmentResponse>
            {
                Items = appointments.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<AppointmentResponse> GetAppointmentByIdAsync(Guid id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                throw new Exception("Appointment not found.");

            return MapToResponse(appointment);
        }

        public async Task<AppointmentResponse> UpdateAppointmentStatusAsync(Guid id, UpdateAppointmentStatusRequest request)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                throw new Exception("Appointment not found.");

            // Chronological status tracking update
            appointment.Status = request.Status;

            _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return MapToResponse(appointment);
        }

        public async Task<bool> DeleteAppointmentAsync(Guid id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                return false;

            _appointmentRepository.Delete(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return true;
        }

        private static AppointmentResponse MapToResponse(Appointment appointment)
        {
            return new AppointmentResponse
            {
                Id = appointment.Id,
                PatientProfileId = appointment.PatientProfileId,
                ScheduledDate = appointment.ScheduledDate,
                Status = appointment.Status,
                LocationArea = appointment.LocationArea,
                Notes = appointment.Notes,
                CreatedAt = appointment.CreatedAt
            };
        }
    }
}