using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitCategories;

public class GetAllSubmitCategoriesQuery :
    PagedRequest
{
    public int SubmitId { get; set; }
    public int FestivalId { get; set; }
    public  RequestSubmitCategoriesType Type { get; set; }
}

public enum RequestSubmitCategoriesType:int
{
    Submit=0,
    Festival=1
}
