using Hisubmit.Client.SharedModels.Wrapper;
using Hisubmit.Client.SharedModels.Features.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.SoldProducts.Queries;

public class GetAllSoldProductQuery :
    PagedRequest
{
    public int? FestivalId { get; set; }
    public string SearchString { get; set; }
    public RequestAccountType RequestAccountType { get; set; }
}