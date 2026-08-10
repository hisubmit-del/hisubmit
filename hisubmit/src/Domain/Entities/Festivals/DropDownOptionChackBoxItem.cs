using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class DropDownOptionCheckBoxItem:AuditableEntity<int>
    {
        public string Title { get; set; }
        public SubmissionQuestion Question { get; set; }
        public int QuestionId { get; set; }
    }
    
}

