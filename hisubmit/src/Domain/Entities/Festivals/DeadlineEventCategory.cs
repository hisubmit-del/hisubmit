using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class DeadlineEventCategory:AuditableEntity<int>
    {
        public double? GoldFee { get; set; }
        public double? StudentFee { get; set; }
        public double? StandardFee { get; set; }

        public int EventCategoryId { get; set; }
        public int DeadLineId { get; set; }

        public EventCategory EventCategory { get; set; }
        public DeadLine DeadLine { get; set; }
    }
}

