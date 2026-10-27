using ClinicManagement.Domain.DTOs;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for doctor operations.
/// </summary>
public interface IDoctorService
{
    Task<DoctorDetailDto?> GetDoctorInfoAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> RejectAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default);
    Task<IEnumerable<BillDto>> GetBillableAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientHistoryDto>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default);
}
