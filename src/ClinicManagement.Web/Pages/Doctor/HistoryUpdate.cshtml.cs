using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class HistoryUpdateModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<HistoryUpdateModel> _logger;

    public HistoryUpdateModel(IDoctorService doctorService, ILogger<HistoryUpdateModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public int AppointmentId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public IActionResult OnGet(int appointmentId, string patientName = "")
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        AppointmentId = appointmentId;
        PatientName = patientName;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId, string disease, string progress, string prescription)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        AppointmentId = appointmentId;

        try
        {
            var result = await _doctorService.UpdatePrescriptionAsync(userId.Value, appointmentId, disease, progress, prescription);
            if (result)
            {
                Message = "Patient history updated successfully!";
                Success = true;
            }
            else
            {
                Message = "Failed to update patient history.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment ID: {AppointmentId}", appointmentId);
            Message = "An error occurred. Please try again.";
        }

        return Page();
    }
}
