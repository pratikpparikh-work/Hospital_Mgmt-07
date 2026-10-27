namespace ClinicManagement.Domain.DTOs;

/// <summary>
/// Result of a login attempt.
/// </summary>
public class LoginResultDto
{
    public bool Success { get; set; }
    public int UserId { get; set; }
    public int UserType { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Result of a patient registration.
/// </summary>
public class RegisterResultDto
{
    public bool Success { get; set; }
    public int PatientId { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// DTO for patient registration.
/// </summary>
public class PatientRegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

/// <summary>
/// DTO for admin home dashboard data.
/// </summary>
public class AdminHomeDto
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentSummaryDto> Departments { get; set; } = new List<DepartmentSummaryDto>();
    public IEnumerable<AppointmentSummaryDto> Appointments { get; set; } = new List<AppointmentSummaryDto>();
}

/// <summary>
/// Summary of a department for admin home.
/// </summary>
public class DepartmentSummaryDto
{
    public string DeptName { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Summary of an appointment for admin home.
/// </summary>
public class AppointmentSummaryDto
{
    public int AppointId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// DTO for doctor list display.
/// </summary>
public class DoctorListDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public double ChargesPerVisit { get; set; }
}

/// <summary>
/// DTO for patient list display.
/// </summary>
public class PatientListDto
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

/// <summary>
/// DTO for staff list display.
/// </summary>
public class StaffListDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>
/// DTO for adding a new doctor.
/// </summary>
public class AddDoctorDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public int DeptNo { get; set; }
    public string Phone { get; set; } = string.Empty;
    public char Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public int WorkExperience { get; set; }
    public int Salary { get; set; }
    public int ChargesPerVisit { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
}

/// <summary>
/// DTO for adding a new staff member.
/// </summary>
public class AddStaffDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public char Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public int Salary { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>
/// Detailed patient information DTO.
/// </summary>
public class PatientDetailDto
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string BirthDate { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
}

/// <summary>
/// Detailed staff information DTO.
/// </summary>
public class StaffDetailDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int Salary { get; set; }
}

/// <summary>
/// Department DTO.
/// </summary>
public class DepartmentDto
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// Bill history entry DTO.
/// </summary>
public class BillHistoryDto
{
    public int AppointId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
}

/// <summary>
/// Current appointment DTO.
/// </summary>
public class CurrentAppointmentDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

/// <summary>
/// Treatment history entry DTO.
/// </summary>
public class TreatmentHistoryDto
{
    public int AppointId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
}

/// <summary>
/// Doctor profile DTO.
/// </summary>
public class DoctorProfileDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Gender { get; set; } = string.Empty;
    public double ChargesPerVisit { get; set; }
    public double ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public int WorkExperience { get; set; }
    public int Age { get; set; }
    public string Department { get; set; } = string.Empty;
}

/// <summary>
/// Free appointment slot DTO.
/// </summary>
public class FreeSlotDto
{
    public int SlotId { get; set; }
    public string Timings { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

/// <summary>
/// Result of booking an appointment.
/// </summary>
public class AppointmentResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Notification DTO for patient.
/// </summary>
public class NotificationDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public bool HasNotification { get; set; }
}

/// <summary>
/// Pending feedback DTO.
/// </summary>
public class PendingFeedbackDto
{
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public bool HasPendingFeedback { get; set; }
}

/// <summary>
/// Appointment DTO for doctor views.
/// </summary>
public class AppointmentDto
{
    public int AppointId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
}

/// <summary>
/// Bill DTO for doctor billing view.
/// </summary>
public class BillDto
{
    public int AppointId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
}

/// <summary>
/// Patient history DTO for doctor view.
/// </summary>
public class PatientHistoryDto
{
    public int AppointId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public string? BillStatus { get; set; }
}

/// <summary>
/// Detailed doctor information DTO.
/// </summary>
public class DoctorDetailDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public double ChargesPerVisit { get; set; }
    public double? MonthlySalary { get; set; }
    public double? ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public int? WorkExperience { get; set; }
    public int Age { get; set; }
    public string BirthDate { get; set; } = string.Empty;
}
