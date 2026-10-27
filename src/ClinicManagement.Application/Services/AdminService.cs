using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for admin operations.
/// </summary>
public class AdminService : IAdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILoginRepository _loginRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IStaffRepository staffRepository,
        IDepartmentRepository departmentRepository,
        IAppointmentRepository appointmentRepository,
        ILoginRepository loginRepository,
        ILogger<AdminService> logger)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _staffRepository = staffRepository;
        _departmentRepository = departmentRepository;
        _appointmentRepository = appointmentRepository;
        _loginRepository = loginRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets admin home dashboard data.
    /// </summary>
    public async Task<AdminHomeDto> GetAdminHomeDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching admin home data");

            var doctors = await _doctorRepository.GetAllActiveAsync(cancellationToken);
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);

            var totalIncome = appointments
                .Where(a => a.BillStatus == "Paid" && a.BillAmount.HasValue)
                .Sum(a => a.BillAmount!.Value);

            var deptSummaries = departments.Select(d => new DepartmentSummaryDto
            {
                DeptName = d.DeptName,
                Description = d.Description,
                DoctorCount = doctors.Count(doc => doc.DeptNo == d.DeptNo)
            });

            var appointmentSummaries = appointments.Take(20).Select(a => new AppointmentSummaryDto
            {
                AppointId = a.AppointId,
                PatientName = a.Patient?.Name ?? "Unknown",
                DoctorName = a.Doctor?.Name ?? "Unknown",
                Date = a.Date,
                Status = GetStatusText(a.AppointmentStatus)
            });

            return new AdminHomeDto
            {
                TotalDoctors = doctors.Count(),
                TotalPatients = patients.Count(),
                TotalIncome = totalIncome,
                Departments = deptSummaries,
                Appointments = appointmentSummaries
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching admin home data");
            return new AdminHomeDto();
        }
    }

    /// <summary>
    /// Gets list of doctors with optional search.
    /// </summary>
    public async Task<IEnumerable<DoctorListDto>> GetDoctorsAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Doctor> doctors;
            if (string.IsNullOrWhiteSpace(searchQuery))
                doctors = await _doctorRepository.GetAllActiveAsync(cancellationToken);
            else
                doctors = await _doctorRepository.SearchAsync(searchQuery, cancellationToken);

            return doctors.Select(d => new DoctorListDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                Department = d.Department?.DeptName ?? string.Empty,
                Phone = d.Phone,
                ChargesPerVisit = d.ChargesPerVisit
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching doctors");
            return Enumerable.Empty<DoctorListDto>();
        }
    }

    /// <summary>
    /// Gets list of patients with optional search.
    /// </summary>
    public async Task<IEnumerable<PatientListDto>> GetPatientsAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Patient> patients;
            if (string.IsNullOrWhiteSpace(searchQuery))
                patients = await _patientRepository.GetAllAsync(cancellationToken);
            else
                patients = await _patientRepository.SearchAsync(searchQuery, cancellationToken);

            return patients.Select(p => new PatientListDto
            {
                PatientId = p.PatientId,
                Name = p.Name,
                Phone = p.Phone
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching patients");
            return Enumerable.Empty<PatientListDto>();
        }
    }

    /// <summary>
    /// Gets list of staff with optional search.
    /// </summary>
    public async Task<IEnumerable<StaffListDto>> GetStaffAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<OtherStaff> staff;
            if (string.IsNullOrWhiteSpace(searchQuery))
                staff = await _staffRepository.GetAllAsync(cancellationToken);
            else
                staff = await _staffRepository.SearchAsync(searchQuery, cancellationToken);

            return staff.Select(s => new StaffListDto
            {
                StaffId = s.StaffId,
                Name = s.Name,
                Designation = s.Designation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching staff");
            return Enumerable.Empty<StaffListDto>();
        }
    }

    /// <summary>
    /// Adds a new doctor to the system.
    /// </summary>
    public async Task<bool> AddDoctorAsync(AddDoctorDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new doctor: {Name}", dto.Name);

            // Create login entry
            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = 2 // Doctor
            };
            var loginId = await _loginRepository.AddAsync(login, cancellationToken);

            // Create doctor entry
            var doctor = new Doctor
            {
                DoctorId = loginId,
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = DateTime.Parse(dto.BirthDate),
                Gender = dto.Gender,
                DeptNo = dto.DeptNo,
                ChargesPerVisit = dto.ChargesPerVisit,
                MonthlySalary = dto.Salary,
                Qualification = dto.Qualification,
                Specialization = dto.Specialization,
                WorkExperience = dto.WorkExperience,
                Status = 1
            };

            await _doctorRepository.AddAsync(doctor, cancellationToken);
            _logger.LogInformation("Doctor added successfully with ID: {DoctorId}", loginId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor: {Name}", dto.Name);
            return false;
        }
    }

    /// <summary>
    /// Adds a new staff member.
    /// </summary>
    public async Task<bool> AddStaffAsync(AddStaffDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new staff: {Name}", dto.Name);

            var staff = new OtherStaff
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = DateTime.TryParse(dto.BirthDate, out var bd) ? bd : null,
                Gender = dto.Gender,
                Salary = dto.Salary,
                HighestQualification = dto.Qualification,
                Designation = dto.Designation
            };

            await _staffRepository.AddAsync(staff, cancellationToken);
            _logger.LogInformation("Staff added successfully: {Name}", dto.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff: {Name}", dto.Name);
            return false;
        }
    }

    /// <summary>
    /// Soft-deletes a doctor (sets status to 0).
    /// </summary>
    public async Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting doctor with ID: {DoctorId}", id);
            return await _doctorRepository.SoftDeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor with ID: {DoctorId}", id);
            return false;
        }
    }

    /// <summary>
    /// Deletes a staff member.
    /// </summary>
    public async Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting staff with ID: {StaffId}", id);
            await _staffRepository.DeleteAsync(id, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff with ID: {StaffId}", id);
            return false;
        }
    }

    /// <summary>
    /// Checks if a doctor email already exists.
    /// </summary>
    public async Task<bool> DoctorEmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _doctorRepository.EmailExistsAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking doctor email: {Email}", email);
            return false;
        }
    }

    /// <summary>
    /// Gets detailed patient information.
    /// </summary>
    public async Task<PatientDetailDto?> GetPatientDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);
            if (patient == null) return null;

            var age = DateTime.Today.Year - patient.BirthDate.Year;
            if (patient.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;

            return new PatientDetailDto
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                Phone = patient.Phone,
                Address = patient.Address,
                BirthDate = patient.BirthDate.ToString("yyyy-MM-dd"),
                Age = age,
                Gender = patient.Gender.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching patient detail for ID: {PatientId}", id);
            return null;
        }
    }

    /// <summary>
    /// Gets detailed staff information.
    /// </summary>
    public async Task<StaffDetailDto?> GetStaffDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            if (staff == null) return null;

            return new StaffDetailDto
            {
                StaffId = staff.StaffId,
                Name = staff.Name,
                Phone = staff.Phone,
                Address = staff.Address,
                Gender = staff.Gender.ToString(),
                Designation = staff.Designation,
                Salary = (int)(staff.Salary ?? 0)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching staff detail for ID: {StaffId}", id);
            return null;
        }
    }

    /// <summary>
    /// Gets all departments.
    /// </summary>
    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            return departments.Select(d => new DepartmentDto
            {
                DeptNo = d.DeptNo,
                DeptName = d.DeptName,
                Description = d.Description
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching departments");
            return Enumerable.Empty<DepartmentDto>();
        }
    }

    private static string GetStatusText(int status) => status switch
    {
        1 => "Approved",
        2 => "Pending",
        3 => "Completed",
        4 => "Rejected",
        _ => "Unknown"
    };
}
