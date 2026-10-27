using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages;

/// <summary>
/// Page model for the login/registration page.
/// </summary>
public class SignUpModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<SignUpModel> _logger;

    public SignUpModel(IAuthService authService, ILogger<SignUpModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [BindProperty]
    public LoginViewModel LoginInput { get; set; } = new();

    [BindProperty]
    public RegisterViewModel RegisterInput { get; set; } = new();

    public string LoginMessage { get; set; } = string.Empty;
    public string RegisterMessage { get; set; } = string.Empty;
    public bool RegisterSuccess { get; set; }

    public void OnGet()
    {
        // Clear session on page load (logout)
        HttpContext.Session.Clear();
    }

    /// <summary>
    /// Handles login form submission.
    /// </summary>
    public async Task<IActionResult> OnPostLoginAsync()
    {
        if (!ModelState.IsValid)
        {
            LoginMessage = "Please fill in all required fields.";
            return Page();
        }

        try
        {
            var result = await _authService.ValidateLoginAsync(LoginInput.Email, LoginInput.Password);

            if (result.Success)
            {
                HttpContext.Session.SetInt32("UserId", result.UserId);
                HttpContext.Session.SetInt32("UserType", result.UserType);

                return result.UserType switch
                {
                    1 => RedirectToPage("/Patient/PatientHome"),
                    2 => RedirectToPage("/Doctor/DoctorHome"),
                    3 => RedirectToPage("/Admin/AdminHome"),
                    _ => Page()
                };
            }

            LoginMessage = result.Message;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            LoginMessage = "There was some error. Try Again!";
            return Page();
        }
    }

    /// <summary>
    /// Handles patient registration form submission.
    /// </summary>
    public async Task<IActionResult> OnPostRegisterAsync()
    {
        if (!ModelState.IsValid)
        {
            RegisterMessage = "Please fill in all required fields correctly.";
            return Page();
        }

        try
        {
            var dto = new PatientRegisterDto
            {
                Name = RegisterInput.Name,
                BirthDate = RegisterInput.BirthDate,
                Email = RegisterInput.Email,
                Password = RegisterInput.Password,
                PhoneNo = RegisterInput.PhoneNo,
                Gender = RegisterInput.Gender,
                Address = RegisterInput.Address
            };

            var result = await _authService.RegisterPatientAsync(dto);

            if (result.Success)
            {
                HttpContext.Session.SetInt32("UserId", result.PatientId);
                HttpContext.Session.SetInt32("UserType", 1);
                return RedirectToPage("/Patient/PatientHome");
            }

            RegisterMessage = result.Message;
            RegisterSuccess = false;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            RegisterMessage = "There was some error. Try again!";
            RegisterSuccess = false;
            return Page();
        }
    }
}
