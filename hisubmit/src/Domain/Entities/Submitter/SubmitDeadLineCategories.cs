using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Submitter
{
    public class SubmitDeadLineCategories:AuditableEntity<int>
    {
        public FeeType FeeType { get; set; }
        public double? Price { get; set; }

        public int SubmitId { get; set; }
        public Submit Submit { get; set; }

        public int DeadlineEventCategoryId { get; set; }
        public DeadlineEventCategory DeadlineEventCategory { get; set; }
    }
}
