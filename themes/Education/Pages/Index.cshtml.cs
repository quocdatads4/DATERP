using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Education.Pages
{
    public class IndexModel : AbpPageModel
    {
        public ActionResult OnGet()
        {
            if (!CurrentUser.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login");
            }

            if (CurrentUser.IsInRole("Administrator"))
            {
                return RedirectToPage("/Admin/Dashboard");
            }

            if (CurrentUser.IsInRole("Student"))
            {
                return RedirectToPage("/Student/Dashboard");
            }

            if (CurrentUser.IsInRole("Teacher"))
            {
                return RedirectToPage("/Teacher/Dashboard");
            }

            // Fallback if user has no specific role or unknown role
            // Can redirect to a generic welcome page or profile
            return Page();
        }
    }
}
