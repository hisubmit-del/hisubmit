using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class CategoryDeadLineFee:AuditableEntity<int>
    {
        public int GoldFee { get; set; }
        public int StudentFee { get; set; }
        public int StandardFee { get; set; }
        public int DeadlineEventCategoryId { get; set; }
        public DeadlineEventCategory DeadlineEventCategory { get; set; }
    }
}


