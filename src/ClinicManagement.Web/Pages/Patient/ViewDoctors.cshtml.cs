using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class ViewDoctorsModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<ViewDoctorsModel> _logger;

    public ViewDoctorsModel(IPatientService patientService, ILogger<ViewDoctorsModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public string SelectedDepartment { get; set; } = string.Empty;
    public IEnumerable<DepartmentOptionViewModel> Departments { get; set; } = new List<DepartmentOptionViewModel>();
    public IEnumerable<DoctorListViewModel> Doctors { get; set; } = new List<DoctorListViewModel>();

    public async Task<IActionResult> OnGetAsync(string? deptName = "")
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        SelectedDepartment = deptName ?? string.Empty;

        try
        {
            var departments = await _patientService.GetDepartmentsAsync();
            Departments = departments.Select(d => new DepartmentOptionViewModel
            {
                DeptNo = d.DeptNo,
                DeptName = d.DeptName
            });

            if (!string.IsNullOrEmpty(SelectedDepartment))
            {
                var doctors = await _patientService.GetDoctorsByDepartmentAsync(SelectedDepartment);
                Doctors = doctors.Select(d => new DoctorListViewModel
                {
                    DoctorId = d.DoctorId,
                    Name = d.Name,
                    Department = d.Department,
                    Phone = d.Phone,
                    ChargesPerVisit = d.ChargesPerVisit
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading view doctors page");
        }

        return Page();
    }
}
