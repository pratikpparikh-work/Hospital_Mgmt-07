using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class AppointmentRequestSentModel : PageModel
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        Success = TempData["AppointmentSuccess"] as bool? ?? false;
        Message = TempData["AppointmentMessage"] as string ?? string.Empty;

        return Page();
    }
}
