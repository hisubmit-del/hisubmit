using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Enums;
using System.Collections.Generic;

namespace Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll
{
    public class GetAllSubmissionQuestionResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Questiontype Questiontype { get; set; }
        public bool ApplyforAllCategory { get; set; }

        public List<UpdateDropDownCheckBoxOption> Options { get; set; }

        public GetAllSubmissionQuestionResponse()
        {
            Options = new List<UpdateDropDownCheckBoxOption>();
        }
    }
}
