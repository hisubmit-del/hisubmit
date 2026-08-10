namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditEventCategory
{
    public class UpdateDeadlineCategoryonFee
    {
        public int Id { get; set; }
        public int EventCategoryId { get; set; }
        public int DeadLineId { get; set; }
        public int? GoldFee { get; set; }
        public int? StudentFee { get; set; }
        public int? StandardFee { get; set; }


        public DateTime? DeadLineDate { get; set; }
        public string DeadLineName { get; set; }
        public string CategoryName { get; set; }
    }
}
