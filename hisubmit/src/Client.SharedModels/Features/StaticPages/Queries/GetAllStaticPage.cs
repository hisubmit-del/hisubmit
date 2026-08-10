using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;
using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.StaticPages.Queries;

public class GetAllStaticPageRequest:PagedRequest
{
    public  bool? IsEnable { get; set; }

    public string SearchString { get; set; }

    public ContentType Type { get; set; }

}


public class GetAllStaticPageResponse
{
    public  int Id { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public bool IsEnable { get; set; }
    public ContentType Type { get; set; }
    public FaqType FaqType { get; set; }

    public bool IsSelected { get; set; }

    public string Content { get; set; }

}
