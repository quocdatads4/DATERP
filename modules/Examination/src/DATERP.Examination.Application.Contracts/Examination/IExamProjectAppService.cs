using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DATERP.Examination.Examination;

public interface IExamProjectAppService :
    ICrudAppService<
        ExamProjectDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateExamProjectDto>
{
    Task<ListResultDto<ExamProjectDto>> GetListByExamListIdAsync(Guid examListId);
}
