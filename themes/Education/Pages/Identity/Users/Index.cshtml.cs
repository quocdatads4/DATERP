using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Identity;

namespace Education.Pages.Identity.Users;

public class IndexModel : AbpPageModel
{
    protected IIdentityUserAppService IdentityUserAppService { get; }

    public IReadOnlyList<IdentityUserDto> Users { get; set; }

    public IndexModel(IIdentityUserAppService identityUserAppService)
    {
        IdentityUserAppService = identityUserAppService;
        Users = new List<IdentityUserDto>();
    }

    public long TotalUsers { get; set; }
    public long ActiveUsers { get; set; }
    public long InactiveUsers { get; set; }
    public long PendingUsers { get; set; } // Mock or map if possible

    public async Task OnGetAsync()
    {
        var result = await IdentityUserAppService.GetListAsync(new GetIdentityUsersInput { MaxResultCount = 1000 });
        Users = result.Items;

        // Calculate Statistics (InMemory for the current page/batch, ideally should be a count query)
        TotalUsers = result.TotalCount;
        ActiveUsers = Users.Count(u => u.IsActive);
        InactiveUsers = Users.Count(u => !u.IsActive);
        PendingUsers = 0; // ABP Identity doesn't have "Pending" state by default
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await IdentityUserAppService.DeleteAsync(id);
        return NoContent();
    }
}
