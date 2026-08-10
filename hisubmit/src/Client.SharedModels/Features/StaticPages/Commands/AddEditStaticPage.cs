using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Seo;

namespace Hisubmit.Client.SharedModels.Features.StaticPages.Commands;

public class AddEditStaticPageRequest 
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public string Content { get; set; }
    public bool IsEnable { get; set; }
    public AddEditSeoTagRequest SeoTag { get; set; } = new();
    public FaqType FaqType { get; set; }
    public ContentType Type { get; set; }
}

public enum ContentType : byte
{
    StaticPage = 0,
    Faq = 1
}

public enum FaqType : byte
{
    General = 0,
    Festival = 1,
    Artist = 2
}
