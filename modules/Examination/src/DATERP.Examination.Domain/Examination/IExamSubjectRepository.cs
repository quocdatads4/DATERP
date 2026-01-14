using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DATERP.Examination.Examination;

public interface IExamSubjectRepository : IRepository<ExamSubject, Guid>
{
    Task<List<ExamSubject>> GetListWithStatsAsync(
        string? filterText = null,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default
    );

    Task<long> GetCountAsync(
        string? filterText = null,
        CancellationToken cancellationToken = default
    );
}
