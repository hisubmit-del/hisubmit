using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class DeadlineEventCategory:AuditableEntity<int>
    {
        public int? GoldFee { get; set; }
        public int? StudentFee { get; set; }
        public int? StandardFee { get; set; }

        public int EventCategoryId { get; set; }
        public int DeadLineId { get; set; }

        public EventCategory EventCategory { get; set; }
        public DeadLine DeadLine { get; set; }
    }
}

