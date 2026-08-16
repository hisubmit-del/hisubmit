using HiSubmit.Application.Features.Judgings.Commands.AddEditJudgiingButton;
using HiSubmit.Application.Features.Judgings.Commands.AddEditJudgingButton;
using HiSubmit.Application.Features.Submission.SubmissionQuestions.Query.GetAll;
using HiSubmit.Domain.Enums;
using System.Collections.Generic;

namespace HiSubmit.Application.Features.Judgings.Queries.Detail
{
    public class GetJudgingDetailResponse
    {
        public int Id { get; set; }

        public ProjectType ProjectType { get; set; }
        public bool AddComment { get; set; }
        public string Comment { get; set; }
        public List<AddEditJudgingButtonCommand> JudgingButtons { get; set; }
        public List<AddEditJudgingFiledCommand> JudgingFileds { get; set; }
        public List<GetAllSubmissionQuestionResponse> Questions { get; set; }
        public int FestivalId { get; set; }

        public GetJudgingDetailResponse()
        {
            JudgingButtons = new List<AddEditJudgingButtonCommand>();
            JudgingFileds = new List<AddEditJudgingFiledCommand>();
            Questions = new List<GetAllSubmissionQuestionResponse>();
        }
    }
}
