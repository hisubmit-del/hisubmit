using HiSubmit.Application.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using HiSubmit.Domain.Enums;
using System.Collections.Generic;

namespace HiSubmit.Application.Features.Submission.SubmissionQuestions.Query.GetAll
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
