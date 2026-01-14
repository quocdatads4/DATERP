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
public class ExamTaskAppService :
    CrudAppService<
        ExamTask,
        ExamTaskDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateExamTaskDto>,
    IExamTaskAppService
{
    private readonly IRepository<ExamProject, Guid> _projectRepository;

    public ExamTaskAppService(
        IRepository<ExamTask, Guid> repository,
        IRepository<ExamProject, Guid> projectRepository)
        : base(repository)
    {
        _projectRepository = projectRepository;
    }

    public override async Task<ExamTaskDto> GetAsync(Guid id)
    {
        var task = await Repository.GetAsync(id);
        var dto = ObjectMapper.Map<ExamTask, ExamTaskDto>(task);

        var project = await _projectRepository.GetAsync(task.ProjectId);
        dto.ProjectName = project.Name;

        return dto;
    }

    public override async Task<PagedResultDto<ExamTaskDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await Repository.GetQueryableAsync();
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = queryable.OrderBy(x => x.Order);
        queryable = queryable.Skip(input.SkipCount).Take(input.MaxResultCount);

        var tasks = await AsyncExecuter.ToListAsync(queryable);
        var dtos = ObjectMapper.Map<List<ExamTask>, List<ExamTaskDto>>(tasks);

        // Load Project names
        var projectIds = dtos.Select(x => x.ProjectId).Distinct().ToList();
        var projects = await _projectRepository.GetListAsync(x => projectIds.Contains(x.Id));
        var projectDict = projects.ToDictionary(x => x.Id, x => x.Name);

        foreach (var dto in dtos)
        {
            dto.ProjectName = projectDict.GetValueOrDefault(dto.ProjectId) ?? "";
        }

        return new PagedResultDto<ExamTaskDto>(totalCount, dtos);
    }

    public async Task<ListResultDto<ExamTaskDto>> GetListByProjectIdAsync(Guid projectId)
    {
        var tasks = await Repository.GetListAsync(x => x.ProjectId == projectId);
        var dtos = ObjectMapper.Map<List<ExamTask>, List<ExamTaskDto>>(tasks.OrderBy(x => x.Order).ToList());

        var project = await _projectRepository.GetAsync(projectId);
        foreach (var dto in dtos)
        {
            dto.ProjectName = project.Name;
        }

        return new ListResultDto<ExamTaskDto>(dtos);
    }
}
