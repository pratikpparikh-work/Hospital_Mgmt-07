using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for patient operations.
/// </summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository,
        IDepartmentRepository departmentRepository,
        IAppointmentRepository appointmentRepository,
        ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _departmentRepository = departmentRepository;
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets patient information.
    /// </summary>
    public async Task<PatientDetailDto?> GetPatientInfoAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
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
            _logger.LogError(ex, "Error fetching patient info for ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Gets bill history for a patient.
    /// </summary>
    public async Task<IEnumerable<BillHistoryDto>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetBillHistoryByPatientIdAsync(patientId, cancellationToken);
            return appointments.Select(a => new BillHistoryDto
            {
                AppointId = a.AppointId,
                DoctorName = a.Doctor?.Name ?? "Unknown",
                Date = a.Date,
                BillAmount = a.BillAmount,
                BillStatus = a.BillStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching bill history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<BillHistoryDto>();
        }
    }

    /// <summary>
    /// Gets current appointment for a patient.
    /// </summary>
    public async Task<CurrentAppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);
            if (appointment == null) return null;

            return new CurrentAppointmentDto
            {
                DoctorName = appointment.Doctor?.Name ?? "Unknown",
                Timings = appointment.Date?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current appointment for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Gets treatment history for a patient.
    /// </summary>
    public async Task<IEnumerable<TreatmentHistoryDto>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetTreatmentHistoryByPatientIdAsync(patientId, cancellationToken);
            return appointments.Select(a => new TreatmentHistoryDto
            {
                AppointId = a.AppointId,
                DoctorName = a.Doctor?.Name ?? "Unknown",
                Date = a.Date,
                Disease = a.Disease,
                Progress = a.Progress,
                Prescription = a.Prescription
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching treatment history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<TreatmentHistoryDto>();
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

    /// <summary>
    /// Gets doctors by department.
    /// </summary>
    public async Task<IEnumerable<DoctorListDto>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctors = await _doctorRepository.GetByDepartmentAsync(deptName, cancellationToken);
            return doctors.Select(d => new DoctorListDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                Department = d.Department?.DeptName ?? deptName,
                Phone = d.Phone,
                ChargesPerVisit = d.ChargesPerVisit
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching doctors for department: {DeptName}", deptName);
            return Enumerable.Empty<DoctorListDto>();
        }
    }

    /// <summary>
    /// Gets doctor profile.
    /// </summary>
    public async Task<DoctorProfileDto?> GetDoctorProfileAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor == null) return null;

            var age = DateTime.Today.Year - doctor.BirthDate.Year;
            if (doctor.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;

            return new DoctorProfileDto
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Gender = doctor.Gender.ToString(),
                ChargesPerVisit = doctor.ChargesPerVisit,
                ReputeIndex = doctor.ReputeIndex ?? 0,
                PatientsTreated = doctor.PatientsTreated,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
                WorkExperience = doctor.WorkExperience ?? 0,
                Age = age,
                Department = doctor.Department?.DeptName ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching doctor profile for ID: {DoctorId}", doctorId);
            return null;
        }
    }

    /// <summary>
    /// Gets free appointment slots for a doctor.
    /// </summary>
    public async Task<IEnumerable<FreeSlotDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var slots = await _appointmentRepository.GetFreeSlotsByDoctorAndPatientAsync(doctorId, patientId, cancellationToken);
            return slots.Select(s => new FreeSlotDto
            {
                SlotId = s.AppointId,
                Timings = s.Date?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty,
                Date = s.Date ?? DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching free slots for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<FreeSlotDto>();
        }
    }

    /// <summary>
    /// Books an appointment.
    /// </summary>
    public async Task<AppointmentResultDto> BookAppointmentAsync(int doctorId, int patientId, int freeSlotId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Booking appointment for patient {PatientId} with doctor {DoctorId}", patientId, doctorId);

            var slot = await _appointmentRepository.GetByIdAsync(freeSlotId, cancellationToken);
            if (slot == null)
            {
                return new AppointmentResultDto { Success = false, Message = "Selected slot not available." };
            }

            slot.PatientId = patientId;
            slot.AppointmentStatus = 2; // Pending
            slot.PatientNotification = 2; // Unseen
            slot.DoctorNotification = 2; // Unseen
            slot.FeedbackStatus = 2; // Pending

            await _appointmentRepository.UpdateAsync(slot, cancellationToken);

            return new AppointmentResultDto { Success = true, Message = "Appointment request sent successfully!" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment for patient ID: {PatientId}", patientId);
            return new AppointmentResultDto { Success = false, Message = "There was some error. Try again!" };
        }
    }

    /// <summary>
    /// Gets notifications for a patient.
    /// </summary>
    public async Task<NotificationDto?> GetNotificationsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = await _appointmentRepository.GetNotificationByPatientIdAsync(patientId, cancellationToken);
            if (notification == null)
            {
                return new NotificationDto { HasNotification = false };
            }

            return new NotificationDto
            {
                HasNotification = true,
                DoctorName = notification.Doctor?.Name ?? "Unknown",
                Timings = notification.Date?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching notifications for patient ID: {PatientId}", patientId);
            return new NotificationDto { HasNotification = false };
        }
    }

    /// <summary>
    /// Gets pending feedback for a patient.
    /// </summary>
    public async Task<PendingFeedbackDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetPendingFeedbackByPatientIdAsync(patientId, cancellationToken);
            if (appointment == null)
            {
                return new PendingFeedbackDto { HasPendingFeedback = false };
            }

            return new PendingFeedbackDto
            {
                HasPendingFeedback = true,
                AppointmentId = appointment.AppointId,
                DoctorName = appointment.Doctor?.Name ?? "Unknown",
                Timings = appointment.Date?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pending feedback for patient ID: {PatientId}", patientId);
            return new PendingFeedbackDto { HasPendingFeedback = false };
        }
    }

    /// <summary>
    /// Submits feedback for an appointment.
    /// </summary>
    public async Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Submitting feedback for appointment ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.FeedbackStatus = 1; // Given
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }
}
