using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class BillsHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<BillsHistoryModel> _logger;

    public BillsHistoryModel(IPatientService patientService, ILogger<BillsHistoryModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public IEnumerable<BillHistoryDto> Bills { get; set; } = new List<BillHistoryDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        try
        {
            Bills = await _patientService.GetBillHistoryAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bill history for user ID: {UserId}", userId);
        }

        return Page();
    }
}
