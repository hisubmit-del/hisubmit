using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllVenue
{
    public class GetAllVenueQuery : PagedRequest
    {
        public int FestivalId { get; set; }

        public string SearchString { get; set; }
    }
}