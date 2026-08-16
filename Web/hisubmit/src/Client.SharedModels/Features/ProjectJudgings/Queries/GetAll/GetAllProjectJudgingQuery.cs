using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetAll;

public class GetAllProjectJudgingQuery : PagedRequest
{
    public int? SubmitId { get; set; }
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    public bool GetCurrentUser { get; set; }
    public string SearchString { get; set; }
}