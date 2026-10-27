using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for adding staff members.
/// </summary>
public class AddStaffModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AddStaffModel> _logger;

    public AddStaffModel(IAdminService adminService, ILogger<AddStaffModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [BindProperty]
    public AddStaffViewModel StaffInput { get; set; } = new();

    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public IActionResult OnGet()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        if (!ModelState.IsValid)
        {
            Message = "Please fill in all required fields.";
            return Page();
        }

        try
        {
            var dto = new AddStaffDto
            {
                Name = StaffInput.Name,
                BirthDate = StaffInput.BirthDate,
                Phone = StaffInput.Phone,
                Gender = StaffInput.Gender,
                Address = StaffInput.Address,
                Salary = StaffInput.Salary,
                Qualification = StaffInput.Qualification,
                Designation = StaffInput.Designation
            };

            var result = await _adminService.AddStaffAsync(dto);

            if (result)
            {
                Message = "Staff member added successfully!";
                Success = true;
                StaffInput = new AddStaffViewModel();
            }
            else
            {
                Message = "Failed to add staff member. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member");
            Message = "An error occurred. Please try again.";
        }

        return Page();
    }
}
