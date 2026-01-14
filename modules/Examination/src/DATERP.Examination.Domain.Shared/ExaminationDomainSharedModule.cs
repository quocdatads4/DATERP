using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using DATERP.Examination.Localization;
using Volo.Abp.Validation.Localization;
using Volo.Abp.Validation;
using Volo.Abp.VirtualFileSystem;

namespace DATERP.Examination;

[DependsOn(
    typeof(AbpValidationModule)
    )]
public class ExaminationDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ExaminationDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<ExaminationResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/Examination");

            options.DefaultResourceType = typeof(ExaminationResource);
        });
    }
}
