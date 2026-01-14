using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace DATERP.Examination.Web.Menus;

public class ExaminationMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        // Menu is managed centrally by DATERP.Web to maintain the "Đào tạo" -> "Quản lý thi cử" structure.
        return Task.CompletedTask;
    }
}
