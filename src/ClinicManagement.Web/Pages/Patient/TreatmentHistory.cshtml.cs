using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TreatmentHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<TreatmentHistoryModel> _logger;

    public TreatmentHistoryModel(IPatientService patientService, ILogger<TreatmentHistoryModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public IEnumerable<TreatmentHistoryDto> Treatments { get; set; } = new List<TreatmentHistoryDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        try
        {
            Treatments = await _patientService.GetTreatmentHistoryAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading treatment history for user ID: {UserId}", userId);
        }

        return Page();
    }
}
