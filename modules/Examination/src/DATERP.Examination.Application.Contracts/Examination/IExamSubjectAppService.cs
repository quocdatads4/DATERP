using System;
using Volo.Abp.Application.Services;

namespace DATERP.Examination.Examination;

public interface IExamSubjectAppService :
    ICrudAppService<
        ExamSubjectDto,
        Guid,
        Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto,
        CreateUpdateExamSubjectDto>
{
    System.Threading.Tasks.Task<ExamSubjectDto> GetByCodeAsync(string code);
}
