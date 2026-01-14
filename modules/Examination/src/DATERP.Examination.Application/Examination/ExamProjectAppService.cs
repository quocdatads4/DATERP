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
public class ExamProjectAppService :
    CrudAppService<
        ExamProject,
        ExamProjectDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateExamProjectDto>,
    IExamProjectAppService
{
    private readonly IRepository<ExamList, Guid> _examListRepository;

    public ExamProjectAppService(
        IRepository<ExamProject, Guid> repository,
        IRepository<ExamList, Guid> examListRepository)
        : base(repository)
    {
        _examListRepository = examListRepository;
    }

    public override async Task<ExamProjectDto> GetAsync(Guid id)
    {
        var project = await Repository.GetAsync(id);
        var dto = ObjectMapper.Map<ExamProject, ExamProjectDto>(project);

        var examList = await _examListRepository.GetAsync(project.ExamListId);
        dto.ExamListTitle = examList.Title;

        return dto;
    }

    public override async Task<PagedResultDto<ExamProjectDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await Repository.GetQueryableAsync();
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = queryable.OrderBy(x => x.Order);
        queryable = queryable.Skip(input.SkipCount).Take(input.MaxResultCount);

        var projects = await AsyncExecuter.ToListAsync(queryable);
        var dtos = ObjectMapper.Map<List<ExamProject>, List<ExamProjectDto>>(projects);

        // Load ExamList titles
        var examListIds = dtos.Select(x => x.ExamListId).Distinct().ToList();
        var examLists = await _examListRepository.GetListAsync(x => examListIds.Contains(x.Id));
        var examListDict = examLists.ToDictionary(x => x.Id, x => x.Title);

        foreach (var dto in dtos)
        {
            dto.ExamListTitle = examListDict.GetValueOrDefault(dto.ExamListId) ?? "";
        }

        return new PagedResultDto<ExamProjectDto>(totalCount, dtos);
    }

    public async Task<ListResultDto<ExamProjectDto>> GetListByExamListIdAsync(Guid examListId)
    {
        var projects = await Repository.GetListAsync(x => x.ExamListId == examListId);
        var dtos = ObjectMapper.Map<List<ExamProject>, List<ExamProjectDto>>(projects.OrderBy(x => x.Order).ToList());

        var examList = await _examListRepository.GetAsync(examListId);
        foreach (var dto in dtos)
        {
            dto.ExamListTitle = examList.Title;
        }

        return new ListResultDto<ExamProjectDto>(dtos);
    }
}
