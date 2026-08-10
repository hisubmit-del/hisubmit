using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Enums;
using System.Collections.Generic;

namespace Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetDetail
{
    public class GetSubmissionQuestionDetailResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Questiontype Questiontype { get; set; }
        public List<UpdateDropDownCheckBoxOption> Options { get; set; }
        public int FestivalId { get; set; }
        
        public bool ApplyforAllCategory { get; set; }

        public List<int> EventCategoryId { get; set; }
    }
}
