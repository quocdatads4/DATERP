using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DATERP.Examination.Localization;
using DATERP.Examination.Web.Menus;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace DATERP.Examination.Web;

[DependsOn(
    typeof(ExaminationApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAutoMapperModule)
    )]
public class ExaminationWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(ExaminationResource), typeof(ExaminationWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(ExaminationWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new ExaminationMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ExaminationWebModule>();
            if (context.Services.GetHostingEnvironment().IsDevelopment())
            {
                options.FileSets.ReplaceEmbeddedByPhysical<ExaminationWebModule>(System.IO.Path.Combine(context.Services.GetHostingEnvironment().ContentRootPath, $@"..\..\modules\Examination\src\DATERP.Examination.Web"));
                options.FileSets.ReplaceEmbeddedByPhysical<ExaminationDomainSharedModule>(System.IO.Path.Combine(context.Services.GetHostingEnvironment().ContentRootPath, $@"..\..\modules\Examination\src\DATERP.Examination.Domain.Shared"));
            }
        });

        context.Services.AddAutoMapperObjectMapper<ExaminationWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ExaminationWebModule>(validate: false);
        });

        Configure<RazorPagesOptions>(options =>
        {
            //Configure authorization.
        });
    }
}
