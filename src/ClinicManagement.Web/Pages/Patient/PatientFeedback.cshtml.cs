using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientFeedbackModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientFeedbackModel> _logger;

    public PatientFeedbackModel(IPatientService patientService, ILogger<PatientFeedbackModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public bool HasPendingFeedback { get; set; }
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public bool FeedbackSubmitted { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        try
        {
            var feedback = await _patientService.GetPendingFeedbackAsync(userId.Value);
            if (feedback != null)
            {
                HasPendingFeedback = feedback.HasPendingFeedback;
                AppointmentId = feedback.AppointmentId;
                DoctorName = feedback.DoctorName;
                Timings = feedback.Timings;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading feedback for user ID: {UserId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        try
        {
            await _patientService.SubmitFeedbackAsync(appointmentId);
            FeedbackSubmitted = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment ID: {AppointmentId}", appointmentId);
        }

        return Page();
    }
}
