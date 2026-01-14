using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace DATERP.Examination.Examination;

public class ExamSessionAppService : ApplicationService, IExamSessionAppService
{
    private readonly IRepository<ExamSession, Guid> _examSessionRepository;

    public ExamSessionAppService(IRepository<ExamSession, Guid> examSessionRepository)
    {
        _examSessionRepository = examSessionRepository;
    }

    public async Task<Guid> StartSessionAsync(StartExamSessionDto input)
    {
        var session = new ExamSession(
            GuidGenerator.Create(),
            CurrentUser.GetId(),
            input.ExamProjectId
        );

        await _examSessionRepository.InsertAsync(session);

        return session.Id;
    }

    public async Task CompleteSessionAsync(CompleteExamSessionDto input)
    {
        var session = await _examSessionRepository.GetAsync(input.SessionId);

        session.Complete(input.FilePath); // Updates Status, EndTime, FilePath

        await _examSessionRepository.UpdateAsync(session);
    }
}
