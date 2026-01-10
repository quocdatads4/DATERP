using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Education.Pages.Student
{
    // [Authorize("DATERP.Permissions.Student.Default")]
    public class DashboardModel : AbpPageModel
    {
        public void OnGet()
        {
        }
    }
}
