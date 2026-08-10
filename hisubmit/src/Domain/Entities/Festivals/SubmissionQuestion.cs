using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;
using System.Collections.Generic;
using HiSubmit.Domain.Entities.Festivals.Tickets;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class SubmissionQuestion : AuditableEntity<int>
    {
        public string Title { get; set; }
        public Questiontype Questiontype { get; set; }
        public List<DropDownOptionCheckBoxItem> Options { get; set; }
        public int? FestivalId { get; set; }
        public Festival Festival { get; set; }
        public bool ApplyforAllCategory { get; set; }
        public int? JudgingId { get; set; }
        public Judging Judging { get; set; }

        
        
        
        public  int? TicketId { get; set; }
        public  Ticket Ticket { get; set; }
        public List<SubmissionQuestionEventCategory> SubmissionQuestionEventCategories { get; set; }


        public SubmissionQuestion()
        {
            SubmissionQuestionEventCategories = new List<SubmissionQuestionEventCategory>();
            Options = new List<DropDownOptionCheckBoxItem>();
        }
    }
}

