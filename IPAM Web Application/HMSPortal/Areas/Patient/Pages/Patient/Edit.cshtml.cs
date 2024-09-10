using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HMSPortal.Areas.Patient.Pages.Patient
{
    public class EditModel : PageModel
    {
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
