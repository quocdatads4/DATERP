using System;
using Volo.Abp.Application.Dtos;

namespace DATERP.Examination.Examination;

public class ExamListDto : FullAuditedEntityDto<Guid>
{
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = default!;
    public string Title { get; set; } = default!;
    public int TimeLimit { get; set; }
    public int Order { get; set; }

    // UI Properties
    public string? Description { get; set; }
    public string? Level { get; set; } // e.g., "Cơ bản", "Nâng cao"
    public string? Duration { get; set; } // e.g., "45 phút", "1 giờ"
    public bool IsFree { get; set; }
    public decimal Price { get; set; }

    // Statistics
    public int TotalProjects { get; set; }
    public int TotalTasks { get; set; }
}

public class CreateUpdateExamListDto
{
    public Guid SubjectId { get; set; }
    public string Title { get; set; } = default!;
    public int TimeLimit { get; set; } = 50;
    public int Order { get; set; }
}
