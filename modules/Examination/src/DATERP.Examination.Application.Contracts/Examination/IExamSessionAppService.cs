using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DATERP.Examination.Examination;

public interface IExamSessionAppService : IApplicationService
{
    Task<Guid> StartSessionAsync(StartExamSessionDto input);
    Task CompleteSessionAsync(CompleteExamSessionDto input);
}
