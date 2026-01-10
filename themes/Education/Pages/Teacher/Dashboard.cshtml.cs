using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Education.Pages.Teacher
{
    // [Authorize("DATERP.Permissions.Teacher.Default")]
    public class DashboardModel : AbpPageModel
    {
        public void OnGet()
        {
        }
    }
}
