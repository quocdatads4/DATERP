using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace DATERP.Examination.Examination;

public class ExamSubmissionAppService : ApplicationService, IExamSubmissionAppService
{
    private readonly IRepository<ExamResult, Guid> _examResultRepository;

    public ExamSubmissionAppService(IRepository<ExamResult, Guid> examResultRepository)
    {
        _examResultRepository = examResultRepository;
    }

    public async Task<ExamResultDto> SubmitScoreAsync(SubmitScoreInput input)
    {
        var result = new ExamResult(
            GuidGenerator.Create(),
            CurrentUser.GetId(),
            input.ExamListId,
            input.TotalScore
        );

        await _examResultRepository.InsertAsync(result);

        return new ExamResultDto
        {
            Id = result.Id,
            TotalScore = result.TotalScore,
            IsPassed = result.IsPassed,
            CompletedAt = result.CompletedAt
        };
    }
}
