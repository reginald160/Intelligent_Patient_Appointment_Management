using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Scripting;

namespace HMSPortal.Areas.Patient.Pages.Patient
{
    public class AddModel : PageModel
    {
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public string PostalCode { get; set; } = string.Empty;
		public string HouseNumber { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public DateTime DateOfBirth { get; set; }
		public string Gender { get; set; } = string.Empty;
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public void OnGet()
        {
        }

		public async Task OnGetAsync(string returnUrl = null)
        {
			returnUrl ??= Url.Content("~/");

		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
			returnUrl ??= Url.Content("~/");
			return Page();
		}

	}
}
