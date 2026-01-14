using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DATERP.Examination.Examination;

public interface IExamSubmissionAppService : IApplicationService
{
    Task<ExamResultDto> SubmitScoreAsync(SubmitScoreInput input);
}
