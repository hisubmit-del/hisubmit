using Hisubmit.Client.SharedModels.Features.Judgings.Commands.AddEditJudgiingButton;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.AddEditJudgingButton;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;
using Hisubmit.Client.SharedModels.Enums;
using System.Collections.Generic;

namespace Hisubmit.Client.SharedModels.Features.Judgings.Queries.Detail
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
