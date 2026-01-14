using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace DATERP.Examination.Examination;

[ConnectionStringName("Default")]
public class ExaminationDbContext : AbpDbContext<ExaminationDbContext>, IExaminationDbContext
{
    public DbSet<ExamSubject> ExamSubjects { get; set; }
    public DbSet<ExamList> ExamLists { get; set; }
    public DbSet<ExamProject> ExamProjects { get; set; }
    public DbSet<ExamTask> ExamTasks { get; set; }
    public DbSet<ExamSession> ExamSessions { get; set; }
    public DbSet<ExamResult> ExamResults { get; set; }

    public ExaminationDbContext(DbContextOptions<ExaminationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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

        builder.Entity<ExamSession>(b =>
        {
            b.ToTable("ExamSessions");
            b.ConfigureByConvention();
            b.Property(x => x.FilePath).HasMaxLength(1024);
        });

        builder.Entity<ExamResult>(b =>
        {
            b.ToTable("ExamResults");
            b.ConfigureByConvention();
        });
    }
}
