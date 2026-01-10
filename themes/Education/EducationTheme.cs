using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.DependencyInjection;

namespace Education.Themes;

[ThemeName(Name)]
public class EducationTheme : ITheme, ITransientDependency
{
    public const string Name = "Education";

    public virtual string GetLayout(string name, bool fallbackToDefault = true)
    {
        switch (name)
        {
            case StandardLayouts.Application:
                return "~/Themes/Education/Layouts/Application.cshtml";
            case StandardLayouts.Account:
                return "~/Themes/Education/Layouts/Account.cshtml";
            case StandardLayouts.Empty:
                return "~/Themes/Education/Layouts/Empty.cshtml";
            default:
                return fallbackToDefault
                    ? "~/Themes/Education/Layouts/Application.cshtml"
                    : null!;
        }
    }
}
