using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class SubmissionQuestionEventCategory : AuditableEntity<int>
    {
        public int EventCategoryId { get; set; }
        public int SubmissionQuestionId { get; set; }
        public SubmissionQuestion SubmissionQuestion { get; set; }
        public EventCategory EventCategory { get; set; }
    }
    
}

