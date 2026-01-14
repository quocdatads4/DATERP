using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace DATERP.Examination;

[DependsOn(
    typeof(ExaminationDomainSharedModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class ExaminationDomainModule : AbpModule
{
}
