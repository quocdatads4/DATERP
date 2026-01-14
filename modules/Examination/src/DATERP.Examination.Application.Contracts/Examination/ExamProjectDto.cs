using System;
using Volo.Abp.Application.Dtos;

namespace DATERP.Examination.Examination;

public class ExamProjectDto : FullAuditedEntityDto<Guid>
{
    public Guid ExamListId { get; set; }
    public string ExamListTitle { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Instruction { get; set; }
    public int Order { get; set; }
}

public class CreateUpdateExamProjectDto
{
    public Guid ExamListId { get; set; }
    public string Name { get; set; } = default!;
    public string? Instruction { get; set; }
    public int Order { get; set; }
}
