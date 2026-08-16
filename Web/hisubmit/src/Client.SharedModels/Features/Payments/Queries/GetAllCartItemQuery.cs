using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.Payments.Queries;

public class GetAllCartItemQuery :
    PagedRequest
{
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    public string SearchString { get; set; }
    public GetCartItemQueryType Type { get; set; }
}

public enum GetCartItemQueryType
{
    User,
    Festival,
    Admin
}
