using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class BillModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<BillModel> _logger;

    public BillModel(IDoctorService doctorService, ILogger<BillModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public IEnumerable<BillDto> Bills { get; set; } = new List<BillDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            Bills = await _doctorService.GetBillableAppointmentsAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bills for doctor ID: {UserId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostMarkPaidAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            await _doctorService.MarkBillPaidAsync(userId.Value, appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as paid for appointment ID: {AppointmentId}", appointmentId);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostMarkUnpaidAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            await _doctorService.MarkBillUnpaidAsync(userId.Value, appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as unpaid for appointment ID: {AppointmentId}", appointmentId);
        }

        return RedirectToPage();
    }
}
