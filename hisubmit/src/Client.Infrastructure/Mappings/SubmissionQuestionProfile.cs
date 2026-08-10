using AutoMapper;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetDetail;

namespace HiSubmit.Client.Infrastructure.Mappings
{
    public class SubmissionQuestionProfile : Profile
    {
        public SubmissionQuestionProfile()
        {
            CreateMap<AddEditSubmissionQuestionCommand, GetSubmissionQuestionDetailResponse>().ReverseMap()
                .ForMember(p => p.EventCategoriesId, map => map.MapFrom(k=>k.EventCategoryId));
        }
    }
}