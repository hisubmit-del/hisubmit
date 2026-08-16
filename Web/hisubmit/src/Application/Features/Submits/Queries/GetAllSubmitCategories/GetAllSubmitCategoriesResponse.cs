using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.Submits.Queries.GetAllSubmitCategories;

public class GetAllSubmitCategoriesResponse
{
    public int Id { get; set; }
    public int Price { get; set; }
    public int SubmitId { get; set; }
    public FeeType FeeType { get; set; }
    public string DeadlineName { get; set; }
    public string EventCategoryName { get; set; }
    public int DeadlineEventCategoryId { get; set; }
}