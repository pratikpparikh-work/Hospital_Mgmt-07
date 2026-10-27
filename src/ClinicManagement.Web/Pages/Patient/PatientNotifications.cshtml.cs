using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientNotificationsModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientNotificationsModel> _logger;

    public PatientNotificationsModel(IPatientService patientService, ILogger<PatientNotificationsModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public bool HasNotification { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        try
        {
            var notification = await _patientService.GetNotificationsAsync(userId.Value);
            if (notification != null)
            {
                HasNotification = notification.HasNotification;
                DoctorName = notification.DoctorName;
                Timings = notification.Timings;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading notifications for user ID: {UserId}", userId);
        }

        return Page();
    }
}
