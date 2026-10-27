using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for doctor registration form.
/// </summary>
public class DoctorRegistrationFormModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<DoctorRegistrationFormModel> _logger;

    public DoctorRegistrationFormModel(IAdminService adminService, ILogger<DoctorRegistrationFormModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [BindProperty]
    public AddDoctorViewModel DoctorInput { get; set; } = new();

    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        await LoadDepartmentsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        await LoadDepartmentsAsync();

        if (!ModelState.IsValid)
        {
            Message = "Please fill in all required fields.";
            return Page();
        }

        try
        {
            // Check if email already exists
            var emailExists = await _adminService.DoctorEmailExistsAsync(DoctorInput.Email);
            if (emailExists)
            {
                Message = "Email already exists. Please use a different email.";
                return Page();
            }

            var dto = new AddDoctorDto
            {
                Name = DoctorInput.Name,
                Email = DoctorInput.Email,
                Password = DoctorInput.Password,
                BirthDate = DoctorInput.BirthDate,
                DeptNo = DoctorInput.DeptNo,
                Phone = DoctorInput.Phone,
                Gender = DoctorInput.Gender,
                Address = DoctorInput.Address,
                WorkExperience = DoctorInput.WorkExperience,
                Salary = DoctorInput.Salary,
                ChargesPerVisit = DoctorInput.ChargesPerVisit,
                Specialization = DoctorInput.Specialization,
                Qualification = DoctorInput.Qualification
            };

            var result = await _adminService.AddDoctorAsync(dto);

            if (result)
            {
                Message = "Doctor registered successfully!";
                Success = true;
                DoctorInput = new AddDoctorViewModel();
                await LoadDepartmentsAsync();
            }
            else
            {
                Message = "Failed to register doctor. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering doctor");
            Message = "An error occurred. Please try again.";
        }

        return Page();
    }

    private async Task LoadDepartmentsAsync()
    {
        var departments = await _adminService.GetDepartmentsAsync();
        DoctorInput.Departments = departments.Select(d => new DepartmentOptionViewModel
        {
            DeptNo = d.DeptNo,
            DeptName = d.DeptName
        });
    }
}
