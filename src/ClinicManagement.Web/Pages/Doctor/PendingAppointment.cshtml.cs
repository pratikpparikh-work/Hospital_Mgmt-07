using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PendingAppointmentModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PendingAppointmentModel> _logger;

    public PendingAppointmentModel(IDoctorService doctorService, ILogger<PendingAppointmentModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending appointments for doctor ID: {UserId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            await _doctorService.ApproveAppointmentAsync(appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment ID: {AppointmentId}", appointmentId);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            await _doctorService.RejectAppointmentAsync(appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting appointment ID: {AppointmentId}", appointmentId);
        }

        return RedirectToPage();
    }
}
