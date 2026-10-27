using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Domain.DTOs;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Page model for patient home page.
/// </summary>
public class PatientHomeModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientHomeModel> _logger;

    public PatientHomeModel(IPatientService patientService, ILogger<PatientHomeModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public PatientDetailDto? PatientInfo { get; set; }
    public CurrentAppointmentViewModel? CurrentAppointment { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
        {
            return RedirectToPage("/SignUp");
        }

        try
        {
            PatientInfo = await _patientService.GetPatientInfoAsync(userId.Value);

            var currentAppt = await _patientService.GetCurrentAppointmentAsync(userId.Value);
            if (currentAppt != null)
            {
                CurrentAppointment = new CurrentAppointmentViewModel
                {
                    DoctorName = currentAppt.DoctorName,
                    Timings = currentAppt.Timings
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient home for user ID: {UserId}", userId);
        }

        return Page();
    }
}
