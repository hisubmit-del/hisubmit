using Hisubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Application.Requests.Catalog
{
    public class GetAllPagedProductsRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public  int? FestivalId { get; set; }
    }
}

