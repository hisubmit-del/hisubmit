using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Seo.GetPAgeSeoTags;

public class GetPageSeoTagsQuery
{
    public PageType  PageType { get; set; }
    public string PageId { get; set; }
}

public class GetPageSeoTagResult:SeoTagDto
{
    
}
