using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Entities.Submitter;
using System.Collections.Generic;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class ProjectJudging : AuditableEntity<int>
    {
        //public int ProjectId { get; set; }
        //public Project Project { get; set; }
        public int SubmitId { get; set; }
        public Submit Submit { get; set; }
        public string UserId { get; set; }    
        public int? JudgingButtonId { get; set; }
        public JudgingButton JudgingButton { get; set; }

        public string Comment { get; set; }
        
        public RefereeStatus RefereeStatus { get; set; }

        public List<JudgingFiledAnswered> JudgingFiledAnswereds { get; set; }
        public List<SubmitAnswerQuestion> SubmitAnswerQuestions { get; set; }
    }


    public class JudgingFiledAnswered : AuditableEntity<int>
    {
        public int Rate { get; set; }

        public JudgingFiled JudgingFiled { get; set; }

        public int JudgingFiledId { get; set; }

        public int ProjectJudgingId { get; set; }
        public ProjectJudging ProjectJudging { get; set; }
    }

}
