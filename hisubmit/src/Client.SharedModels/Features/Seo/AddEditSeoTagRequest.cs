using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Seo;

public class AddEditSeoTagRequest 
{
    public string PageId { get; set; }
    public string PageTitle { get; set; }
    public PageType Type { get; set; }

    //Tags
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Index { get; set; } = true;
    public bool Follow { get; set; } = true;
    public string CanonicalUrl { get; set; }
    public string MetaKeywords { get; set; }

}

