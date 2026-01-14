using System;
using Volo.Abp.Application.Dtos;

namespace DATERP.Examination.Examination;

public class ExamSubjectDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }

    // UI Properties
    public string ColorClass { get; set; } = "primary";
    public string Icon { get; set; } = "ti-book";
    public string? BadgeText { get; set; }
    public string? BadgeIcon { get; set; }
    public string? Title { get; set; }

    // Stats Properties
    public string Duration { get; set; } = "0h";
    public int TotalLessons { get; set; } = 0; // Legacy, map to ExamLists?
    public int TotalExams { get; set; } = 0; // Legacy, map to ExamProjects?

    public int TotalExamLists { get; set; }
    public int TotalExamProjects { get; set; }
    public int TotalExamTasks { get; set; }
}

public class CreateUpdateExamSubjectDto
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
}
