using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace DATERP.Examination;

[DependsOn(
    typeof(ExaminationDomainModule),
    typeof(ExaminationApplicationContractsModule),
    typeof(AbpAutoMapperModule)
)]
public class ExaminationApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<ExaminationApplicationAutoMapperProfile>(validate: true);
        });
    }
}
