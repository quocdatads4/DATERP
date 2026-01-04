using DATERP.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;
using Volo.Abp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Timing;
using System;

namespace DATERP.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(DATERPEntityFrameworkCoreModule),
    typeof(DATERPApplicationContractsModule)
    )]
public class DATERPDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);

        var configuration = context.Services.GetConfiguration();
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = configuration.GetConnectionString("Default");
        });

        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Utc;
        });
    }
}
