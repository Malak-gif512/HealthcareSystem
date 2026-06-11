using HealthcareSystem.Application.DTOs;
using HealthcareSystem.Application.DTOs.ClinicalRecords;
using HealthcareSystem.Application.Interfaces;
using HealthcareSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthcareSystem.Application.Services
{
    // Implements business logic for clinical records ensuring data integrity
    public class ClinicalRecordService : IClinicalRecordService
    {
        private readonly IGenericRepository<ClinicalRecord> _recordRepository;
        private readonly IGenericRepository<PatientProfile> _patientRepository;

        public ClinicalRecordService(
            IGenericRepository<ClinicalRecord> recordRepository,
            IGenericRepository<PatientProfile> patientRepository)
        {
            _recordRepository = recordRepository;
            _patientRepository = patientRepository;
        }

        public async Task<ClinicalRecordResponse> CreateRecordAsync(CreateClinicalRecordRequest request)
        {
            // 1. Validate that the target patient profile actually exists
            var patient = await _patientRepository.GetByIdAsync(request.PatientProfileId);
            if (patient == null)
                throw new Exception("Patient profile not found.");

            // 2. Map DTO to Entity
            var record = new ClinicalRecord
            {
                PatientProfileId = request.PatientProfileId,
                Diagnosis = request.Diagnosis,
                ClinicalNotes = request.ClinicalNotes
            };

            // 3. Save to database
            await _recordRepository.AddAsync(record);
            await _recordRepository.SaveChangesAsync();

            return MapToResponse(record);
        }

        public async Task<PagedResult<ClinicalRecordResponse>> GetRecordsByPatientIdAsync(Guid patientProfileId, int pageNumber, int pageSize)
        {
            var query = _recordRepository.GetQueryable()
                .Where(r => r.PatientProfileId == patientProfileId);

            var totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ClinicalRecordResponse>
            {
                Items = records.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ClinicalRecordResponse> GetRecordByIdAsync(Guid id)
        {
            var record = await _recordRepository.GetByIdAsync(id);
            if (record == null)
                throw new Exception("Clinical record not found.");

            return MapToResponse(record);
        }

        public async Task<ClinicalRecordResponse> UpdateRecordAsync(Guid id, CreateClinicalRecordRequest request)
        {
            var record = await _recordRepository.GetByIdAsync(id);
            if (record == null)
                throw new Exception("Clinical record not found.");

            // Update only allowed fields (PatientProfileId should not be changeable)
            record.Diagnosis = request.Diagnosis;
            record.ClinicalNotes = request.ClinicalNotes;

            _recordRepository.Update(record);
            await _recordRepository.SaveChangesAsync();

            return MapToResponse(record);
        }

        public async Task<bool> DeleteRecordAsync(Guid id)
        {
            var record = await _recordRepository.GetByIdAsync(id);
            if (record == null)
                return false;

            _recordRepository.Delete(record);
            await _recordRepository.SaveChangesAsync();

            return true;
        }

        // Helper method for consistent mapping
        private static ClinicalRecordResponse MapToResponse(ClinicalRecord record)
        {
            return new ClinicalRecordResponse
            {
                Id = record.Id,
                PatientProfileId = record.PatientProfileId,
                Diagnosis = record.Diagnosis,
                ClinicalNotes = record.ClinicalNotes,
                CreatedAt = record.CreatedAt
            };
        }
    }
}