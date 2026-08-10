using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class Judging:AuditableEntity<int>
    {
        public ProjectType ProjectType { get; set; }
        public bool AddComment { get; set; }
        public string Comment { get; set; }

        public List<JudgingButton> JudgingButtons { get; set; } 
        public List<JudgingFiled> JudgingFileds { get; set; }
        public List<SubmissionQuestion> SubmissionQuestions { get; set; }

        public int FestivalId { get; set; }
        public Festival Festival { get; set; }
    }
    public class JudgingFiled:AuditableEntity<int>
    {
        public string Name { get; set; }
        public int JudgingId { get; set; }
        public Judging Judging { get; set; }
    }
    public class JudgingButton:AuditableEntity<int>
    {
        public string Name { get; set; }
        public int JudgingId { get; set; }
        public Judging Judging { get; set; }
    }

}
