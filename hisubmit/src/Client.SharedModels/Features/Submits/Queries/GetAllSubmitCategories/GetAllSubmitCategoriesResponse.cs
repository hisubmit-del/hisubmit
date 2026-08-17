using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitCategories;

public class GetAllSubmitCategoriesResponse
{
    public int Id { get; set; }
    public double Price { get; set; }
    public int SubmitId { get; set; }
    public FeeType FeeType { get; set; }
    public string DeadlineName { get; set; }
    public string EventCategoryName { get; set; }
    public int DeadlineEventCategoryId { get; set; }
}
