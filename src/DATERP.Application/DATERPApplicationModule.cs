using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Account;
using Volo.Abp.TenantManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

namespace DATERP;

[DependsOn(
    typeof(DATERPDomainModule),
    typeof(DATERPApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule)
    )]
public class DATERPApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<DATERPApplicationModule>();
        });
    }
}
