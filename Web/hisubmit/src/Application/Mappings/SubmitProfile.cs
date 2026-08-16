using AutoMapper;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Application.Requests.AnswerQuestions;
using HiSubmit.Application.Features.Submits.Commands;
using HiSubmit.Application.Features.ProjectJudgings.Queries.GetDetail;
using HiSubmit.Application.Features.Submits.Queries.GetAllSubmitCategories;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using DeadLineCategoryDto = HiSubmit.Application.Features.Submits.Queries.GetAllSubmitsQueries.DeadLineCategoryDto;

namespace HiSubmit.Application.Mappings;

public class SubmitProfile:Profile
{
    public SubmitProfile()
    {
        CreateMap<AddSubmitCommand, Submit>().ReverseMap();
        CreateMap<GetAllSubmitsResponse, Submit>().ReverseMap()
            .ForMember(des=>des.ProjectTitle,map=>map.MapFrom(src=>src.Project.Title))
            .ForMember(des=>des.ProjectFileURl,map=>map.MapFrom(src=>src.Project.FileURl))
            .ForMember(des=>des.ProjectUrl,map=>map.MapFrom(src=>src.Project.URL))
            .ForMember(des=>des.FestivalName,map=>map.MapFrom(src=>src.Festival.Name))
            .ForMember(des=>des.FestivalLogoUrl,map=>map.MapFrom(src=>src.Festival.LogoURL))
            .ForMember(des=>des.ProjectOwnerId,map=>map.MapFrom(src=>src.Project.UserId))
            .ForMember(des=>des.ProjectEnglishBriefSynopsis,map=>map.MapFrom(src=>src.Project.EnglishBriefSynopsis))
            .ForMember(des=>des.ProjectOwnerFullName,
                map=>map.MapFrom(src=>src.Project.FirstName + " " + src.Project.LastName ));
        
        CreateMap<AnswerQuestionDto, SubmitAnswerQuestion>().ReverseMap();
        CreateMap<SubmitDto, Submit>().ReverseMap();
            
        CreateMap<DeadlineEventCategory, DeadLineCategoryDto>()
            .ForMember(des=>des.EventCategoryName,map=>map.MapFrom(src=>src.EventCategory.Name)).ReverseMap();

        CreateMap<SubmitDeadLineCategories, GetAllSubmitCategoriesResponse>()
            .ForMember(des => des.EventCategoryName,
                map => map.MapFrom(src => src.DeadlineEventCategory.EventCategory.Name))
            .ForMember(des => des.DeadlineName, map => map.MapFrom(src => src.DeadlineEventCategory.DeadLine.Name));
    }
}