using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for managing clinic (doctors, patients, staff).
/// </summary>
public class ManageClinicModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<ManageClinicModel> _logger;

    public ManageClinicModel(IAdminService adminService, ILogger<ManageClinicModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public string SearchQuery { get; set; } = string.Empty;
    public string ActiveTab { get; set; } = "doctors";
    public IEnumerable<DoctorListViewModel> Doctors { get; set; } = new List<DoctorListViewModel>();
    public IEnumerable<PatientListViewModel> Patients { get; set; } = new List<PatientListViewModel>();
    public IEnumerable<StaffListViewModel> Staff { get; set; } = new List<StaffListViewModel>();

    public async Task<IActionResult> OnGetAsync(string? searchQuery = "", string? tab = "doctors")
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        SearchQuery = searchQuery ?? string.Empty;
        ActiveTab = tab ?? "doctors";

        await LoadDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int id)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        try
        {
            await _adminService.DeleteDoctorAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor ID: {Id}", id);
        }

        return RedirectToPage(new { tab = "doctors" });
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int id)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        try
        {
            await _adminService.DeleteStaffAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff ID: {Id}", id);
        }

        return RedirectToPage(new { tab = "staff" });
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var doctors = await _adminService.GetDoctorsAsync(SearchQuery);
            Doctors = doctors.Select(d => new DoctorListViewModel
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                Department = d.Department,
                Phone = d.Phone,
                ChargesPerVisit = d.ChargesPerVisit
            });

            var patients = await _adminService.GetPatientsAsync(SearchQuery);
            Patients = patients.Select(p => new PatientListViewModel
            {
                PatientId = p.PatientId,
                Name = p.Name,
                Phone = p.Phone
            });

            var staff = await _adminService.GetStaffAsync(SearchQuery);
            Staff = staff.Select(s => new StaffListViewModel
            {
                StaffId = s.StaffId,
                Name = s.Name,
                Designation = s.Designation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading manage clinic data");
        }
    }
}
