using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.Products.Queries.GetAllPaged;

public class GetAllProductsRequest :PagedRequest
{
    
    
    public  int? FestivalId { get; set; }
    public string SearchString { get; set; }
    public  bool? IsEnable { get; set; }
    public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

    public GetAllProductsRequest()
    {
            
    }
    public GetAllProductsRequest(int pageNumber, int pageSize, string searchString, string orderBy,int? festivalId)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchString = searchString;
        FestivalId = festivalId;
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            OrderBy = orderBy.Split(',');
        }
    }
}
