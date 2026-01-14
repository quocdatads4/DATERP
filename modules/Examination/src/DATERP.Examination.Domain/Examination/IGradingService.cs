using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace DATERP.Examination.Examination;

public interface IGradingService : IDomainService
{
    Task<int> GradeProjectAsync(Guid projectId, byte[] fileContent);
}
