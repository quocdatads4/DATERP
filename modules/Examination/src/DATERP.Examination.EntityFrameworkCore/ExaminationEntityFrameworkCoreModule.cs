using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using DATERP.Examination.Examination;

namespace DATERP.Examination;

[DependsOn(
    typeof(ExaminationDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class ExaminationEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<ExaminationDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}
