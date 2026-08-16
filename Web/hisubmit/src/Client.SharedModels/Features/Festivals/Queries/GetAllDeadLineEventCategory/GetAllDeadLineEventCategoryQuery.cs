namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLineEventCategory;

public class GetAllDeadLineEventCategoryQuery 
{
    public int? DeadLineId { get; set; }
    public int FestivalId { get; set; }
    public bool TakeCurrentDeadLine { get; set; }
    public bool SpecfyWithProject { get; set; }

    public int? ProjectId { get; set; }
    public bool? Nearest { get; set; }
}
