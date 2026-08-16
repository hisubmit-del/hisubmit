using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;

namespace HiSubmit.Domain.Entities.Submitter
{
    public class SubmitAnswerQuestion:AuditableEntity<int>
    {
        public string Answer { get; set; }

        //navigation property
        public  int? SubmitId { get; set; }
        public Submit Submit { get; set; }


        public int? ProjectJudgingId { get; set; }
        public ProjectJudging ProjectJudging { get; set; }

        public SubmissionQuestion SubmissionQuestion { get; set; }
        public int SubmissionQuestionId { get; set; }
    }
}
