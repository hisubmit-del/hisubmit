using Hisubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Application.Requests.Documents
{
    public class GetAllPagedDocumentsRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}