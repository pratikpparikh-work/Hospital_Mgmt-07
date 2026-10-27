using ClinicManagement.Domain.DTOs;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for patient operations.
/// </summary>
public interface IPatientService
{
    Task<PatientDetailDto?> GetPatientInfoAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BillHistoryDto>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<CurrentAppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TreatmentHistoryDto>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<DoctorListDto>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken cancellationToken = default);
    Task<DoctorProfileDto?> GetDoctorProfileAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FreeSlotDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default);
    Task<AppointmentResultDto> BookAppointmentAsync(int doctorId, int patientId, int freeSlotId, CancellationToken cancellationToken = default);
    Task<NotificationDto?> GetNotificationsAsync(int patientId, CancellationToken cancellationToken = default);
    Task<PendingFeedbackDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default);
    Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default);
}
