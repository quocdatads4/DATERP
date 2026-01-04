using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.EntityFrameworkCore;
using DATERP.EntityFrameworkCore;
using DATERP;

using Volo.Abp.Account.Web;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.Identity.Web;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.FeatureManagement.Web;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;

namespace DATERP.Web;

[DependsOn(
    typeof(DATERPHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpAccountWebModule),
    typeof(AbpPermissionManagementWebModule),
    typeof(AbpSettingManagementWebModule),
    typeof(AbpSwashbuckleModule),
    typeof(DATERPEntityFrameworkCoreModule),
    typeof(DATERPApplicationModule)
    )]
public class DATERPWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "DATERP API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseConfiguredEndpoints();
    }
}
