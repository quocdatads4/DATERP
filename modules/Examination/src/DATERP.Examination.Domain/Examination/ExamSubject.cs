using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace DATERP.Examination.Examination;

public class ExamSubject : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<ExamList> ExamLists { get; set; } = new List<ExamList>();

    protected ExamSubject() { }

    internal ExamSubject(Guid id, string name, string code, string? description = null) : base(id)
    {
        Name = name;
        Code = code;
        Description = description;
    }
}
