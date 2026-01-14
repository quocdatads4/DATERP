using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace DATERP.Examination.Examination;

public class ExamSubjectRepository : EfCoreRepository<IExaminationDbContext, ExamSubject, Guid>, IExamSubjectRepository
{
    public ExamSubjectRepository(IDbContextProvider<IExaminationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<ExamSubject>> GetListWithStatsAsync(
        string? filterText = null,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        // Eager load the graph to avoid N+1 in Application Layer
        query = query.Include(x => x.ExamLists)
                     .ThenInclude(l => l.ExamProjects)
                     .ThenInclude(p => p.ExamTasks);

        query = query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.Contains(filterText!) || e.Code.Contains(filterText!));

        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? "Name asc" : sorting);

        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public async Task<long> GetCountAsync(string? filterText = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.Contains(filterText!) || e.Code.Contains(filterText!));
        return await query.LongCountAsync(cancellationToken);
    }
}
