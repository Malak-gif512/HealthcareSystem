using HealthcareSystem.Application.DTOs;
using HealthcareSystem.Application.DTOs.Patients;
using HealthcareSystem.Application.Interfaces;
using HealthcareSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthcareSystem.Application.Services
{
    // Implements business logic for patient management with pagination and filtering
    public class PatientService : IPatientService
    {
        private readonly IGenericRepository<PatientProfile> _patientRepository;
        private readonly IGenericRepository<User> _userRepository;

        public PatientService(
            IGenericRepository<PatientProfile> patientRepository,
            IGenericRepository<User> userRepository)
        {
            _patientRepository = patientRepository;
            _userRepository = userRepository;
        }

        public async Task<PatientResponse> CreatePatientAsync(CreatePatientRequest request)
        {
            // 1. Validate that the user exists and has the 'Patient' role
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null || user.Role != Domain.Enums.UserRole.Patient)
                throw new Exception("Invalid User ID or the user does not have the Patient role.");

            // 2. Map DTO to Entity
            var patient = new PatientProfile
            {
                UserId = request.UserId,
                FullName = request.FullName,
                DateOfBirth = request.DateOfBirth,
                BloodType = request.BloodType
            };

            // 3. Save to database
            await _patientRepository.AddAsync(patient);
            await _patientRepository.SaveChangesAsync();

            return MapToResponse(patient);
        }

        public async Task<PagedResult<PatientResponse>> GetPatientsAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            var query = _patientRepository.GetQueryable();

            // Applying optimized filtering conditionally
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.FullName.Contains(searchTerm) || p.BloodType.Contains(searchTerm));
            }

            // Calculating total records for pagination metadata
            var totalCount = await query.CountAsync();

            // Applying pagination safely (Skip & Take translates to OFFSET & FETCH in SQL)
            var patients = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<PatientResponse>
            {
                Items = patients.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PatientResponse> GetPatientByIdAsync(Guid id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new Exception("Patient profile not found.");

            return MapToResponse(patient);
        }

        public async Task<PatientResponse> UpdatePatientAsync(Guid id, CreatePatientRequest request)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new Exception("Patient profile not found.");

            // Update allowed fields
            patient.FullName = request.FullName;
            patient.DateOfBirth = request.DateOfBirth;
            patient.BloodType = request.BloodType;

            _patientRepository.Update(patient);
            await _patientRepository.SaveChangesAsync();

            return MapToResponse(patient);
        }

        public async Task<bool> DeletePatientAsync(Guid id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return false;

            // This will trigger the Soft Delete we configured earlier, not a hard delete!
            _patientRepository.Delete(patient);
            await _patientRepository.SaveChangesAsync();

            return true;
        }

        // Helper method to map entity to response DTO securely
        private static PatientResponse MapToResponse(PatientProfile patient)
        {
            return new PatientResponse
            {
                Id = patient.Id,
                UserId = patient.UserId,
                FullName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                BloodType = patient.BloodType
            };
        }
    }
}