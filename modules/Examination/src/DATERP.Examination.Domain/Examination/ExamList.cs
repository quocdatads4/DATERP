using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace DATERP.Examination.Examination;

public class ExamList : FullAuditedAggregateRoot<Guid>
{
    public Guid SubjectId { get; set; }
    public string Title { get; set; } = default!;
    public int TimeLimit { get; set; } // Phút
    public int Order { get; set; }

    public ICollection<ExamProject> ExamProjects { get; set; } = new List<ExamProject>();

    protected ExamList() { }

    public ExamList(Guid id, Guid subjectId, string title, int timeLimit = 50, int order = 0) : base(id)
    {
        SubjectId = subjectId;
        Title = title;
        TimeLimit = timeLimit;
        Order = order;
    }
}
