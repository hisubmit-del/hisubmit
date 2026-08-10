using Hisubmit.Client.SharedModels.Wrapper;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Reviews.Queries;

public class GetAllReviewQuery : PagedRequest
{
    public int? FestivalId { get; set; }
    public  string SearchString { get; set; }
    public string UserId { get; set; }
    public  CommentType Type { get; set; }
}

public class GetAllReviewResponse
{
    public int Id { get; set; }
    public string UserFullName { get; set; }
    public string UserImages { get; set; }
    public int Rate { get; set; }
    public string Text { get; set; }
    public int FestivalId { get; set; }
    public  string FestivalName { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedOn { get; set; }
}
