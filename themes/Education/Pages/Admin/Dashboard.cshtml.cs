using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Education.Pages.Admin
{
    // [Authorize("DATERP.Permissions.Admin.Default")] // Tạm thời comment để test, sau này sẽ bật
    public class DashboardModel : AbpPageModel
    {
        public void OnGet()
        {
        }
    }
}
