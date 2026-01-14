using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace DATERP.Examination.Examination;

public class ExamProject : FullAuditedEntity<Guid>
{
    public Guid ExamListId { get; set; }
    public string Name { get; set; } = default!;
    public string? Instruction { get; set; }
    public int Order { get; set; }

    public ICollection<ExamTask> ExamTasks { get; set; } = new List<ExamTask>();

    protected ExamProject() { }

    public ExamProject(Guid id, Guid examListId, string name, string? instruction = null, int order = 0) : base(id)
    {
        ExamListId = examListId;
        Name = name;
        Instruction = instruction;
        Order = order;
    }
}
