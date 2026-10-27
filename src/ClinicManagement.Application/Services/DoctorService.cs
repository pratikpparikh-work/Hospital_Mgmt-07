using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for doctor operations.
/// </summary>
public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IAppointmentRepository appointmentRepository,
        ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets doctor information.
    /// </summary>
    public async Task<DoctorDetailDto?> GetDoctorInfoAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor == null) return null;

            var age = DateTime.Today.Year - doctor.BirthDate.Year;
            if (doctor.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;

            return new DoctorDetailDto
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Address = doctor.Address,
                Gender = doctor.Gender.ToString(),
                Department = doctor.Department?.DeptName ?? string.Empty,
                ChargesPerVisit = doctor.ChargesPerVisit,
                MonthlySalary = doctor.MonthlySalary,
                ReputeIndex = doctor.ReputeIndex,
                PatientsTreated = doctor.PatientsTreated,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
                WorkExperience = doctor.WorkExperience,
                Age = age,
                BirthDate = doctor.BirthDate.ToString("yyyy-MM-dd")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching doctor info for ID: {DoctorId}", doctorId);
            return null;
        }
    }

    /// <summary>
    /// Gets pending appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetPendingByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(a => new AppointmentDto
            {
                AppointId = a.AppointId,
                PatientName = a.Patient?.Name ?? "Unknown",
                Date = a.Date,
                Status = "Pending",
                Disease = a.Disease,
                Progress = a.Progress,
                Prescription = a.Prescription
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pending appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Gets today's appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(a => new AppointmentDto
            {
                AppointId = a.AppointId,
                PatientName = a.Patient?.Name ?? "Unknown",
                Date = a.Date,
                Status = GetStatusText(a.AppointmentStatus),
                Disease = a.Disease,
                Progress = a.Progress,
                Prescription = a.Prescription
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching today's appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Approves an appointment.
    /// </summary>
    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving appointment ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.AppointmentStatus = 1; // Approved
            appointment.PatientNotification = 2; // Unseen
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Rejects/deletes an appointment.
    /// </summary>
    public async Task<bool> RejectAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Rejecting appointment ID: {AppointmentId}", appointmentId);
            await _appointmentRepository.DeleteAsync(appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Updates prescription for an appointment.
    /// </summary>
    public async Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating prescription for appointment ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId) return false;

            appointment.Disease = disease;
            appointment.Progress = progress;
            appointment.Prescription = prescription;
            appointment.AppointmentStatus = 3; // Completed

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Gets billable appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<BillDto>> GetBillableAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetBillableByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(a => new BillDto
            {
                AppointId = a.AppointId,
                PatientName = a.Patient?.Name ?? "Unknown",
                Date = a.Date,
                BillAmount = a.BillAmount,
                BillStatus = a.BillStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching billable appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<BillDto>();
        }
    }

    /// <summary>
    /// Marks a bill as paid.
    /// </summary>
    public async Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking bill as paid for appointment ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId) return false;

            appointment.BillStatus = "Paid";
            appointment.AppointmentStatus = 3; // Completed
            appointment.FeedbackStatus = 2; // Pending

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as paid for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Marks a bill as unpaid.
    /// </summary>
    public async Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking bill as unpaid for appointment ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId) return false;

            appointment.BillStatus = "Unpaid";
            appointment.AppointmentStatus = 3; // Completed

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as unpaid for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Gets patient history for a doctor.
    /// </summary>
    public async Task<IEnumerable<PatientHistoryDto>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            return appointments
                .Where(a => a.AppointmentStatus == 3) // Completed
                .Select(a => new PatientHistoryDto
                {
                    AppointId = a.AppointId,
                    PatientName = a.Patient?.Name ?? "Unknown",
                    Date = a.Date,
                    Disease = a.Disease,
                    Progress = a.Progress,
                    Prescription = a.Prescription,
                    BillStatus = a.BillStatus
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching patient history for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<PatientHistoryDto>();
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
