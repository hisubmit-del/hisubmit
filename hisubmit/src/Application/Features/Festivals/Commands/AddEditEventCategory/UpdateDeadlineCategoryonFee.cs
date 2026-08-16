using System;

namespace HiSubmit.Application.Features.Festivals.Commands.AddEditEventCategory
{
    public class UpdateDeadlineCategoryonFee
    {
        public int Id { get; set; }
        public int EventCategoryId { get; set; }
        public int DeadLineId { get; set; }
        public double? GoldFee { get; set; }
        public double? StudentFee { get; set; }
        public double? StandardFee { get; set; }

        public DateTime? DeadLineDate { get; set; }
        public string DeadLineName { get; set; }
        public string CategoryName { get; set; }
    }
}
