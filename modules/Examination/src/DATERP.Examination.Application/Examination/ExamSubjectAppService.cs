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
public class ExamSubjectAppService : ApplicationService, IExamSubjectAppService
{
    private readonly IExamSubjectRepository _examSubjectRepository;
    private readonly ExamSubjectManager _examSubjectManager;

    public ExamSubjectAppService(
        IExamSubjectRepository examSubjectRepository,
        ExamSubjectManager examSubjectManager)
    {
        _examSubjectRepository = examSubjectRepository;
        _examSubjectManager = examSubjectManager;
    }

    public async Task<PagedResultDto<ExamSubjectDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var totalCount = await _examSubjectRepository.GetCountAsync();
        var subjects = await _examSubjectRepository.GetListWithStatsAsync(
            null, // input doesn't have Filter property
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount
        );

        var dtos = subjects.Select(subject =>
        {
            var dto = ObjectMapper.Map<ExamSubject, ExamSubjectDto>(subject);

            // Calculate stats in memory from loaded graph
            dto.TotalExamLists = subject.ExamLists.Count;
            dto.TotalExamProjects = subject.ExamLists.Sum(l => l.ExamProjects.Count);
            dto.TotalExamTasks = subject.ExamLists.SelectMany(l => l.ExamProjects).Sum(p => p.ExamTasks.Count);

            // Map legacy props
            dto.TotalLessons = dto.TotalExamLists;
            dto.TotalExams = dto.TotalExamProjects;

            return dto;
        }).ToList();

        return new PagedResultDto<ExamSubjectDto>(
            totalCount,
            dtos
        );
    }

    public async Task<ExamSubjectDto> GetAsync(Guid id)
    {
        var subject = await _examSubjectRepository.GetAsync(id);
        return ObjectMapper.Map<ExamSubject, ExamSubjectDto>(subject);
    }

    public async Task<ExamSubjectDto> CreateAsync(CreateUpdateExamSubjectDto input)
    {
        var subject = await _examSubjectManager.CreateAsync(
            input.Name,
            input.Code,
            input.Description
        );

        await _examSubjectRepository.InsertAsync(subject);

        return ObjectMapper.Map<ExamSubject, ExamSubjectDto>(subject);
    }

    public async Task<ExamSubjectDto> UpdateAsync(Guid id, CreateUpdateExamSubjectDto input)
    {
        var subject = await _examSubjectRepository.GetAsync(id);

        await _examSubjectManager.UpdateAsync(
            subject,
            input.Name,
            input.Description
        );
        // Code is usually immutable or requires special check in manager if changed

        await _examSubjectRepository.UpdateAsync(subject);

        return ObjectMapper.Map<ExamSubject, ExamSubjectDto>(subject);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _examSubjectRepository.DeleteAsync(id);
    }

    public async Task<ExamSubjectDto> GetByCodeAsync(string code)
    {
        var subject = await _examSubjectRepository.FindAsync(x => x.Code == code);
        if (subject == null)
        {
            throw new UserFriendlyException($"Exam Subject with code '{code}' not found.");
        }
        return ObjectMapper.Map<ExamSubject, ExamSubjectDto>(subject);
    }
}
