namespace HiSubmit.Application.Features.Submits.Queries.GetAllSubmitsQueries;

public class DeadLineCategoryDto
{
    public int Id { get; set; }
    public double GoldFee { get; set; }
    public double StudentFee { get; set; }
    public double StandardFee { get; set; }
    public int EventCategoryId { get; set; }
    public int DeadLineId { get; set; }
    public string EventCategoryName { get; set; }
    public string DeadLineName { get; set; }
}
