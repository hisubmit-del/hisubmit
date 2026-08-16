namespace Hisubmit.Client.SharedModels.Features.Seo;

public class SeoTagDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool  Index { get; set; }
    public bool Follow { get; set; }

    public string CanonicalUrl { get; set; }
    public string MetaKeywords { get; set; }
}