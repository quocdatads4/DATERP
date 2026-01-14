using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DATERP.Examination.Examination;

public interface IExamTaskAppService :
    ICrudAppService<
        ExamTaskDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateExamTaskDto>
{
    Task<ListResultDto<ExamTaskDto>> GetListByProjectIdAsync(Guid projectId);
}
