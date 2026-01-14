using System;

namespace DATERP.Examination.Examination;

public class CompleteExamSessionDto
{
    public Guid SessionId { get; set; }
    public string FilePath { get; set; } = string.Empty;
}
