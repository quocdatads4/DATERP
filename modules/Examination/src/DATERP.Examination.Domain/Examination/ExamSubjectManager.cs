using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp;

namespace DATERP.Examination.Examination;

public class ExamSubjectManager : DomainService
{
    private readonly IExamSubjectRepository _examSubjectRepository;

    public ExamSubjectManager(IExamSubjectRepository examSubjectRepository)
    {
        _examSubjectRepository = examSubjectRepository;
    }

    public async Task<ExamSubject> CreateAsync(string name, string code, string? description = null)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNullOrWhiteSpace(code, nameof(code));

        var existingSubject = await _examSubjectRepository.FindAsync(x => x.Code == code);
        if (existingSubject != null)
        {
            throw new UserFriendlyException($"Exam Subject code '{code}' already exists.");
        }

        return new ExamSubject(GuidGenerator.Create(), name, code, description);
    }

    public async Task UpdateAsync(ExamSubject examSubject, string name, string? description = null)
    {
        Check.NotNull(examSubject, nameof(examSubject));
        Check.NotNullOrWhiteSpace(name, nameof(name));

        examSubject.Name = name;
        examSubject.Description = description;

        await Task.CompletedTask;
    }
}
