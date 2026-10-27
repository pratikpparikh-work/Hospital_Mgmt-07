using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.ViewModels;

/// <summary>
/// ViewModel for the login form.
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for the patient registration form.
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(30, ErrorMessage = "Name cannot exceed 30 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth date is required")]
    public string BirthDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(15, ErrorMessage = "Phone cannot exceed 15 characters")]
    public string PhoneNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(40, ErrorMessage = "Address cannot exceed 40 characters")]
    public string Address { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for admin home dashboard.
/// </summary>
public class AdminHomeViewModel
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentSummaryViewModel> Departments { get; set; } = new List<DepartmentSummaryViewModel>();
    public IEnumerable<AppointmentSummaryViewModel> Appointments { get; set; } = new List<AppointmentSummaryViewModel>();
}

public class DepartmentSummaryViewModel
{
    public string DeptName { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public string? Description { get; set; }
}

public class AppointmentSummaryViewModel
{
    public int AppointId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for doctor registration form.
/// </summary>
public class AddDoctorViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(30)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(30, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth date is required")]
    public string BirthDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required")]
    public int DeptNo { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    public char Gender { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [StringLength(30)]
    public string Address { get; set; } = string.Empty;

    [Range(0, 50, ErrorMessage = "Work experience must be between 0 and 50")]
    public int WorkExperience { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Salary must be positive")]
    public int Salary { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Charges must be positive")]
    public int ChargesPerVisit { get; set; }

    [StringLength(30)]
    public string Specialization { get; set; } = string.Empty;

    [Required(ErrorMessage = "Qualification is required")]
    [StringLength(100)]
    public string Qualification { get; set; } = string.Empty;

    public IEnumerable<DepartmentOptionViewModel> Departments { get; set; } = new List<DepartmentOptionViewModel>();
}

public class DepartmentOptionViewModel
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for staff registration form.
/// </summary>
public class AddStaffViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(30)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth date is required")]
    public string BirthDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    public char Gender { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [StringLength(50)]
    public string Address { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Salary { get; set; }

    [StringLength(50)]
    public string Qualification { get; set; } = string.Empty;

    [Required(ErrorMessage = "Designation is required")]
    [StringLength(15)]
    public string Designation { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for manage clinic page.
/// </summary>
public class ManageClinicViewModel
{
    public string SearchQuery { get; set; } = string.Empty;
    public string ActiveTab { get; set; } = "doctors";
    public IEnumerable<DoctorListViewModel> Doctors { get; set; } = new List<DoctorListViewModel>();
    public IEnumerable<PatientListViewModel> Patients { get; set; } = new List<PatientListViewModel>();
    public IEnumerable<StaffListViewModel> Staff { get; set; } = new List<StaffListViewModel>();
}

public class DoctorListViewModel
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public double ChargesPerVisit { get; set; }
}

public class PatientListViewModel
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

public class StaffListViewModel
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for patient home page.
/// </summary>
public class PatientHomeViewModel
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string BirthDate { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public CurrentAppointmentViewModel? CurrentAppointment { get; set; }
}

public class CurrentAppointmentViewModel
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for bill history page.
/// </summary>
public class BillHistoryViewModel
{
    public IEnumerable<BillHistoryItemViewModel> Bills { get; set; } = new List<BillHistoryItemViewModel>();
}

public class BillHistoryItemViewModel
{
    public int AppointId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
}

/// <summary>
/// ViewModel for treatment history page.
/// </summary>
public class TreatmentHistoryViewModel
{
    public IEnumerable<TreatmentHistoryItemViewModel> Treatments { get; set; } = new List<TreatmentHistoryItemViewModel>();
}

public class TreatmentHistoryItemViewModel
{
    public int AppointId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
}

/// <summary>
/// ViewModel for view doctors page.
/// </summary>
public class ViewDoctorsViewModel
{
    public string SelectedDepartment { get; set; } = string.Empty;
    public IEnumerable<DepartmentOptionViewModel> Departments { get; set; } = new List<DepartmentOptionViewModel>();
    public IEnumerable<DoctorListViewModel> Doctors { get; set; } = new List<DoctorListViewModel>();
}

/// <summary>
/// ViewModel for doctor profile page.
/// </summary>
public class DoctorProfileViewModel
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
/// ViewModel for take appointment page.
/// </summary>
public class TakeAppointmentViewModel
{
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public IEnumerable<FreeSlotViewModel> FreeSlots { get; set; } = new List<FreeSlotViewModel>();
}

public class FreeSlotViewModel
{
    public int SlotId { get; set; }
    public string Timings { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

/// <summary>
/// ViewModel for appointment request sent page.
/// </summary>
public class AppointmentRequestViewModel
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for patient notifications page.
/// </summary>
public class PatientNotificationsViewModel
{
    public bool HasNotification { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for patient feedback page.
/// </summary>
public class PatientFeedbackViewModel
{
    public bool HasPendingFeedback { get; set; }
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public bool FeedbackSubmitted { get; set; }
}

/// <summary>
/// ViewModel for doctor home page.
/// </summary>
public class DoctorHomeViewModel
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

/// <summary>
/// ViewModel for pending appointments page.
/// </summary>
public class PendingAppointmentViewModel
{
    public IEnumerable<AppointmentItemViewModel> Appointments { get; set; } = new List<AppointmentItemViewModel>();
}

public class AppointmentItemViewModel
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
/// ViewModel for patient history update page.
/// </summary>
public class HistoryUpdateViewModel
{
    public int AppointmentId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public IEnumerable<AppointmentItemViewModel> TodaysAppointments { get; set; } = new List<AppointmentItemViewModel>();
}

/// <summary>
/// ViewModel for bill generation page.
/// </summary>
public class BillViewModel
{
    public IEnumerable<BillItemViewModel> Bills { get; set; } = new List<BillItemViewModel>();
}

public class BillItemViewModel
{
    public int AppointId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
}

/// <summary>
/// ViewModel for previous patient history page.
/// </summary>
public class PreviousHistoryViewModel
{
    public IEnumerable<PatientHistoryItemViewModel> History { get; set; } = new List<PatientHistoryItemViewModel>();
}

public class PatientHistoryItemViewModel
{
    public int AppointId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public string? BillStatus { get; set; }
}
