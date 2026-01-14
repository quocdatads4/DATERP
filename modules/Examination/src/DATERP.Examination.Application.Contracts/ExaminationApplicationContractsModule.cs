using Volo.Abp.Modularity;

namespace DATERP.Examination;

[DependsOn(
    typeof(ExaminationDomainSharedModule)
)]
public class ExaminationApplicationContractsModule : AbpModule
{
}
