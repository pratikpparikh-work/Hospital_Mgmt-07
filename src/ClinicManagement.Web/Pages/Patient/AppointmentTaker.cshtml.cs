using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class AppointmentTakerModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<AppointmentTakerModel> _logger;

    public AppointmentTakerModel(IPatientService patientService, ILogger<AppointmentTakerModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public IEnumerable<FreeSlotViewModel> FreeSlots { get; set; } = new List<FreeSlotViewModel>();

    public async Task<IActionResult> OnGetAsync(int doctorId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        DoctorId = doctorId;

        try
        {
            var profile = await _patientService.GetDoctorProfileAsync(doctorId);
            DoctorName = profile?.Name ?? string.Empty;

            var slots = await _patientService.GetFreeSlotsAsync(doctorId, userId.Value);
            FreeSlots = slots.Select(s => new FreeSlotViewModel
            {
                SlotId = s.SlotId,
                Timings = s.Timings,
                Date = s.Date
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading appointment taker for doctor ID: {DoctorId}", doctorId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int doctorId, int slotId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        try
        {
            var result = await _patientService.BookAppointmentAsync(doctorId, userId.Value, slotId);
            TempData["AppointmentMessage"] = result.Message;
            TempData["AppointmentSuccess"] = result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment");
            TempData["AppointmentMessage"] = "An error occurred. Please try again.";
            TempData["AppointmentSuccess"] = false;
        }

        return RedirectToPage("/Patient/AppointmentRequestSent");
    }
}
