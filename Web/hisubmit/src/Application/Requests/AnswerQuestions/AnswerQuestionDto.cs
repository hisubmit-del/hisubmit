using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Application.Requests.AnswerQuestions
{
    public class AnswerQuestionDto
    {

        public int Id { get; set; }
        public int SubmissionQuestionId { get; set; }

        //public int? JudgingId { get; set; }
        //public int? ProductFestivalId { get; set; }

        public string Answer { get; set; }
    }  
}
