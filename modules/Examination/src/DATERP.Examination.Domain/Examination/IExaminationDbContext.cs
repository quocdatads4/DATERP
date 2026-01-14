using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace DATERP.Examination.Examination;

[ConnectionStringName("Default")]
public interface IExaminationDbContext : IEfCoreDbContext
{
    DbSet<ExamSubject> ExamSubjects { get; }
    DbSet<ExamList> ExamLists { get; }
    DbSet<ExamProject> ExamProjects { get; }
    DbSet<ExamTask> ExamTasks { get; }
}
