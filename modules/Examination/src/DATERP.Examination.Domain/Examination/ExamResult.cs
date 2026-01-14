using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace DATERP.Examination.Examination;

public class ExamResult : AuditedAggregateRoot<Guid>
{
    public Guid UserId { get; set; }
    public Guid ExamListId { get; set; }
    public int TotalScore { get; set; } // 0 - 1000
    public bool IsPassed { get; set; }
    public DateTime CompletedAt { get; set; }

    protected ExamResult()
    {
    }

    public ExamResult(Guid id, Guid userId, Guid examListId, int totalScore)
        : base(id)
    {
        UserId = userId;
        ExamListId = examListId;
        TotalScore = totalScore;
        IsPassed = totalScore >= 700;
        CompletedAt = DateTime.Now;
    }
}
