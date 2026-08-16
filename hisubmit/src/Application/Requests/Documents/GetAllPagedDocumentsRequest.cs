using Hisubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Application.Requests.Documents
{
    public class GetAllPagedDocumentsRequest : PagedRequest
    {
        public new string SearchString { get; set; }
    }
}
