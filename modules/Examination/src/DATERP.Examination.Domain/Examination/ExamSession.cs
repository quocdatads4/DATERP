using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace DATERP.Examination.Examination;

public class ExamSession : AuditedAggregateRoot<Guid>
{
    public Guid UserId { get; set; }
    public Guid ExamProjectId { get; set; }
    public string? FilePath { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ExamSessionStatus Status { get; set; }

    protected ExamSession()
    {
    }

    public ExamSession(Guid id, Guid userId, Guid examProjectId)
        : base(id)
    {
        UserId = userId;
        ExamProjectId = examProjectId;
        StartTime = DateTime.Now;
        Status = ExamSessionStatus.InProgress;
    }

    public void Complete(string filePath)
    {
        FilePath = filePath;
        EndTime = DateTime.Now;
        Status = ExamSessionStatus.Completed;
    }
}
