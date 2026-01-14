using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DATERP.Examination.Examination;

public interface IExamListAppService :
    ICrudAppService<
        ExamListDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateExamListDto>
{
    Task<ListResultDto<ExamListDto>> GetListBySubjectIdAsync(Guid subjectId);
}
