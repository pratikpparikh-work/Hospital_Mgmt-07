using ClinicManagement.Domain.DTOs;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for admin operations.
/// </summary>
public interface IAdminService
{
    Task<AdminHomeDto> GetAdminHomeDataAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorListDto>> GetDoctorsAsync(string searchQuery = "", CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientListDto>> GetPatientsAsync(string searchQuery = "", CancellationToken cancellationToken = default);
    Task<IEnumerable<StaffListDto>> GetStaffAsync(string searchQuery = "", CancellationToken cancellationToken = default);
    Task<bool> AddDoctorAsync(AddDoctorDto dto, CancellationToken cancellationToken = default);
    Task<bool> AddStaffAsync(AddStaffDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DoctorEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<PatientDetailDto?> GetPatientDetailAsync(int id, CancellationToken cancellationToken = default);
    Task<StaffDetailDto?> GetStaffDetailAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
}
