using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace DATERP.Examination.Examination;

public class ExamTask : FullAuditedEntity<Guid>
{
    public Guid ProjectId { get; set; }
    public string Content { get; set; } = default!;
    public double Point { get; set; }
    public int Order { get; set; }
    public string? GradingConfig { get; set; }

    protected ExamTask() { }

    public ExamTask(Guid id, Guid projectId, string content, double point = 0, int order = 0, string? gradingConfig = null) : base(id)
    {
        ProjectId = projectId;
        Content = content;
        Point = point;
        Order = order;
        GradingConfig = gradingConfig;
    }
}
