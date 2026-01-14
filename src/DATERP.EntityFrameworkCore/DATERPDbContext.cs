using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using DATERP.Examination.Examination;

namespace DATERP.EntityFrameworkCore;

[ReplaceDbContext(typeof(IBackgroundJobsDbContext))]
[ReplaceDbContext(typeof(IExaminationDbContext))]
[ConnectionStringName("Default")]
public class DATERPDbContext :
        AbpDbContext<DATERPDbContext>,
        IIdentityDbContext,
        ITenantManagementDbContext,
        IBackgroundJobsDbContext,
        IExaminationDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * to replace them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    // Background Jobs
    public DbSet<BackgroundJobRecord> BackgroundJobs { get; set; }

    #endregion

    public DATERPDbContext(DbContextOptions<DATERPDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(DATERPConsts.DbTablePrefix + "YourEntities", DATERPConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        builder.Entity<ExamSubject>(b =>
        {
            b.ToTable("ExamSubjects");
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.Code).IsRequired().HasMaxLength(32);
            b.HasMany(x => x.ExamLists).WithOne().HasForeignKey(x => x.SubjectId).IsRequired();
        });

        builder.Entity<ExamList>(b =>
        {
            b.ToTable("ExamLists");
            b.ConfigureByConvention();
            b.Property(x => x.Title).IsRequired().HasMaxLength(256);
            b.HasMany(x => x.ExamProjects).WithOne().HasForeignKey(x => x.ExamListId).IsRequired();
        });

        builder.Entity<ExamProject>(b =>
        {
            b.ToTable("ExamProjects");
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);
            b.HasMany(x => x.ExamTasks).WithOne().HasForeignKey(x => x.ProjectId).IsRequired();
        });

        builder.Entity<ExamTask>(b =>
        {
            b.ToTable("ExamTasks");
            b.ConfigureByConvention();
            b.Property(x => x.Content).IsRequired();
        });
    }

    public DbSet<ExamSubject> ExamSubjects { get; set; }
    public DbSet<ExamList> ExamLists { get; set; }
    public DbSet<ExamProject> ExamProjects { get; set; }
    public DbSet<ExamTask> ExamTasks { get; set; }
}
