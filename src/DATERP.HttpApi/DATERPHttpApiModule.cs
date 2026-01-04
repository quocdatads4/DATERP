using Localization.Resources.AbpUi;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Account;
using Volo.Abp.TenantManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;

namespace DATERP;

[DependsOn(
    typeof(DATERPApplicationContractsModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpTenantManagementHttpApiModule),
    typeof(AbpAccountHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule)
    )]
public class DATERPHttpApiModule : AbpModule
{

}
