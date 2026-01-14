using AutoMapper;
using Volo.Abp.Application.Dtos;
using DATERP.Examination.Examination;

namespace DATERP.Examination;

public class ExaminationApplicationAutoMapperProfile : Profile
{
    public ExaminationApplicationAutoMapperProfile()
    {
        // ExamSubject mappings
        CreateMap<ExamSubject, ExamSubjectDto>(MemberList.None);
        CreateMap<CreateUpdateExamSubjectDto, ExamSubject>(MemberList.None);
        CreateMap<ExamSubjectDto, CreateUpdateExamSubjectDto>(MemberList.None);

        // ExamList mappings
        CreateMap<ExamList, ExamListDto>(MemberList.None)
            .ForMember(dest => dest.SubjectName, opt => opt.Ignore());
        CreateMap<CreateUpdateExamListDto, ExamList>(MemberList.None);
        CreateMap<ExamListDto, CreateUpdateExamListDto>(MemberList.None);

        // ExamProject mappings
        CreateMap<ExamProject, ExamProjectDto>(MemberList.None)
            .ForMember(dest => dest.ExamListTitle, opt => opt.Ignore());
        CreateMap<CreateUpdateExamProjectDto, ExamProject>(MemberList.None);
        CreateMap<ExamProjectDto, CreateUpdateExamProjectDto>(MemberList.None);

        // ExamTask mappings
        CreateMap<ExamTask, ExamTaskDto>(MemberList.None)
            .ForMember(dest => dest.ProjectName, opt => opt.Ignore());
        CreateMap<CreateUpdateExamTaskDto, ExamTask>(MemberList.None);
        CreateMap<ExamTaskDto, CreateUpdateExamTaskDto>(MemberList.None);
    }
}
