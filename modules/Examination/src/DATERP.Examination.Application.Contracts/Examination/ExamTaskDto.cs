using System;
using Volo.Abp.Application.Dtos;

namespace DATERP.Examination.Examination;

public class ExamTaskDto : FullAuditedEntityDto<Guid>
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = default!;
    public string Content { get; set; } = default!;
    public double Point { get; set; }
    public int Order { get; set; }
    public string? GradingConfig { get; set; }
}

public class CreateUpdateExamTaskDto
{
    public Guid ProjectId { get; set; }
    public string Content { get; set; } = default!;
    public double Point { get; set; }
    public int Order { get; set; }
    public string? GradingConfig { get; set; }
}
