using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin home dashboard.
/// </summary>
public class AdminHomeModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminHomeModel> _logger;

    public AdminHomeModel(IAdminService adminService, ILogger<AdminHomeModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public AdminHomeViewModel AdminData { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
        {
            return RedirectToPage("/SignUp");
        }

        try
        {
            var data = await _adminService.GetAdminHomeDataAsync();

            AdminData = new AdminHomeViewModel
            {
                TotalDoctors = data.TotalDoctors,
                TotalPatients = data.TotalPatients,
                TotalIncome = data.TotalIncome,
                Departments = data.Departments.Select(d => new DepartmentSummaryViewModel
                {
                    DeptName = d.DeptName,
                    DoctorCount = d.DoctorCount,
                    Description = d.Description
                }),
                Appointments = data.Appointments.Select(a => new AppointmentSummaryViewModel
                {
                    AppointId = a.AppointId,
                    PatientName = a.PatientName,
                    DoctorName = a.DoctorName,
                    Date = a.Date,
                    Status = a.Status
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin home data");
        }

        return Page();
    }
}
