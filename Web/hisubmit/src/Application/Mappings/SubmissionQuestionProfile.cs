using AutoMapper;
using HiSubmit.Application.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using HiSubmit.Application.Features.Submission.SubmissionQuestions.Query.GetAll;
using HiSubmit.Application.Features.Submission.SubmissionQuestions.Query.GetDetail;
using HiSubmit.Domain.Entities.Festivals;
using System.Linq;

namespace HiSubmit.Application.Mappings
{
    public class SubmissionQuestionProfile : Profile
    {
        public SubmissionQuestionProfile()
        {
            CreateMap<AddEditSubmissionQuestionCommand, SubmissionQuestion>().ReverseMap();
            CreateMap<UpdateDropDownCheckBoxOption,DropDownOptionCheckBoxItem>().ReverseMap();

            CreateMap<GetAllSubmissionQuestionResponse, SubmissionQuestion>().ReverseMap();

            CreateMap<GetSubmissionQuestionDetailResponse,SubmissionQuestion>().ReverseMap()
                .ForMember(p=>p.EventCategoryId,map=>map
                .MapFrom(p=>p.SubmissionQuestionEventCategories.Select(p=>p.EventCategoryId)));
        }
    }

}