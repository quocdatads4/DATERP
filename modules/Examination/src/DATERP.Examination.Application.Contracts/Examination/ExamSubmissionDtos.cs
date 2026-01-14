using System;
using System.Collections.Generic;

namespace DATERP.Examination.Examination;

public class SubmitScoreInput
{
    public Guid ExamListId { get; set; }
    public int TotalScore { get; set; }
    public bool IsPassed { get; set; }
}

public class ExamResultDto
{
    public Guid Id { get; set; }
    public int TotalScore { get; set; }
    public bool IsPassed { get; set; }
    public DateTime CompletedAt { get; set; }
}
