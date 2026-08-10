using AutoMapper;
using HiSubmit.Application.Features.ProjectJudgings.Commands;
using HiSubmit.Application.Features.ProjectJudgings.Commands.AddProjectJudgingResult;
using HiSubmit.Application.Features.ProjectJudgings.Queries.CheckPermissionForJudging;
using HiSubmit.Application.Features.ProjectJudgings.Queries.GetAll;
using HiSubmit.Domain.Entities.Festivals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiSubmit.Application.Features.ProjectJudgings.Queries.GetDetail;

namespace HiSubmit.Application.Mappings
{
    public class ProjectJudgingProfile:Profile
    {
        public ProjectJudgingProfile()
        {
            CreateMap<AddEditProjectJudgingCommand, ProjectJudging>();
            CreateMap<GetAllProjectJudgingResponse, ProjectJudging>()
                .ReverseMap()
                .ForMember(des=>des.FestivalId,map=>map.MapFrom(src=>src.Submit.FestivalId))
                .ForMember(des=>des.FestivalName,map=>map.MapFrom(src=>src.Submit.Festival.Name))
                .ForMember(des=>des.ProjectName,map=>map.MapFrom(src=>src.Submit.Project.Title))
                .ForMember(des=>des.ProjectId,map=>map.MapFrom(src=>src.Submit.ProjectId))
                .ForMember(des=>des.JudgingButtonName,map=>map.MapFrom(src=>src.JudgingButton.Name))
                .ForMember(des=>des.JudgingFiledAverage,map=>map.MapFrom(src=>TryAverage(src.JudgingFiledAnswereds)))
                .ForMember(des=>des.ProjectURL,map=>map.MapFrom(src=>src.Submit.Project.URL))
                .ForMember(des=>des.ProjectOwner,map=>map.MapFrom(src=>src.Submit.Project.FirstName + " " + src.Submit.Project.LastName))
                .ForMember(des=>des.ProjectFileUrl,map=>map.MapFrom(src=>src.Submit.Project.FileURl))
                ;

            CreateMap<ProjectJudgingDto, ProjectJudging>().ReverseMap()
                .ForMember(des => des.FestivalId, map => map.MapFrom(src => src.Submit.FestivalId));

            CreateMap<ProjectJudging, AddEditProjectJudgingResultCommand>().ReverseMap();
            CreateMap<JudgingFiled, JudgingFieldAnswerDto>().ReverseMap();
            CreateMap<JudgingFiledAnswered, JudgingFieldAnswerDto>().ReverseMap();

            CreateMap<GetProjectJudgingDetailResponse, ProjectJudging>().ReverseMap();
        }

        private static double TryAverage(List<JudgingFiledAnswered> answers)
        {
            if (!answers.Any())
                return 0;

            return
                answers.Sum(p => p.Rate) / answers.Count;
        }
    }
}
