using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DATERP.Examination.Examination;

[RemoteService(Name = "Examination")]
public class ExamListAppService :
    CrudAppService<
        ExamList,
        ExamListDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateExamListDto>,
    IExamListAppService
{
    private readonly IRepository<ExamSubject, Guid> _subjectRepository;

    public ExamListAppService(
        IRepository<ExamList, Guid> repository,
        IRepository<ExamSubject, Guid> subjectRepository)
        : base(repository)
    {
        _subjectRepository = subjectRepository;
    }

    public override async Task<ExamListDto> GetAsync(Guid id)
    {
        var examList = await Repository.GetAsync(id);
        var dto = ObjectMapper.Map<ExamList, ExamListDto>(examList);

        var subject = await _subjectRepository.GetAsync(examList.SubjectId);
        dto.SubjectName = subject.Name;

        return dto;
    }

    public override async Task<PagedResultDto<ExamListDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await Repository.GetQueryableAsync();
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = queryable.OrderBy(x => x.Order);
        queryable = queryable.Skip(input.SkipCount).Take(input.MaxResultCount);

        var examLists = await AsyncExecuter.ToListAsync(queryable);
        var dtos = ObjectMapper.Map<List<ExamList>, List<ExamListDto>>(examLists);

        // Load subject names
        var subjectIds = dtos.Select(x => x.SubjectId).Distinct().ToList();
        var subjects = await _subjectRepository.GetListAsync(x => subjectIds.Contains(x.Id));
        var subjectDict = subjects.ToDictionary(x => x.Id, x => x.Name);

        foreach (var dto in dtos)
        {
            dto.SubjectName = subjectDict.GetValueOrDefault(dto.SubjectId) ?? "";
        }

        return new PagedResultDto<ExamListDto>(totalCount, dtos);
    }

    public async Task<ListResultDto<ExamListDto>> GetListBySubjectIdAsync(Guid subjectId)
    {
        var examLists = await Repository.GetListAsync(x => x.SubjectId == subjectId);
        var dtos = ObjectMapper.Map<List<ExamList>, List<ExamListDto>>(examLists.OrderBy(x => x.Order).ToList());

        var subject = await _subjectRepository.GetAsync(subjectId);
        foreach (var dto in dtos)
        {
            dto.SubjectName = subject.Name;
        }

        return new ListResultDto<ExamListDto>(dtos);
    }
}
