using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PatientHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PatientHistoryModel> _logger;

    public PatientHistoryModel(IDoctorService doctorService, ILogger<PatientHistoryModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            Appointments = await _doctorService.GetTodaysAppointmentsAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading today's appointments for doctor ID: {UserId}", userId);
        }

        return Page();
    }
}
