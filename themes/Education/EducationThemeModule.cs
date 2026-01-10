using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using Education.Bundling;
using Education.Themes;
using Education.Localization;
using Volo.Abp.Localization;

namespace Education;

[DependsOn(
    typeof(AbpAspNetCoreMvcUiThemeSharedModule)
)]
public class EducationThemeModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(EducationThemeModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Configure Virtual File System - StaticWebAssets handles RCL wwwroot automatically
        // No explicit VFS mapping needed for _content/Education paths
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<EducationThemeModule>();
        });

        // Configure Theming - đăng ký Education theme
        Configure<AbpThemingOptions>(options =>
        {
            options.Themes.Add<EducationTheme>();
            options.DefaultThemeName = EducationTheme.Name;
        });

        // Configure Bundling - định nghĩa custom style và script bundles
        Configure<AbpBundlingOptions>(options =>
        {
            // Style Bundle
            options.StyleBundles.Add("Education.Global.Styles", bundle =>
            {
                bundle.AddContributors(typeof(EducationThemeGlobalStyleContributor));
            });

            // Script Bundle
            options.ScriptBundles.Add("Education.Global.Scripts", bundle =>
            {
                bundle.AddContributors(typeof(EducationThemeGlobalScriptContributor));
            });
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<EducationResource>("en")
                .AddVirtualJson("/Localization/Education")
                .AddBaseTypes(typeof(Volo.Abp.Identity.Localization.IdentityResource));
        });
    }
}
