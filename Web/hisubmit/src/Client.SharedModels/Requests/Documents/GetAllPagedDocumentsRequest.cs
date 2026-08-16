using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Requests.Documents;

public class GetAllPagedDocumentsRequest : PagedRequest
{
    public string SearchString { get; set; }
}