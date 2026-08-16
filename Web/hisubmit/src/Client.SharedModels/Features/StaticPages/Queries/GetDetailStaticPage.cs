
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;


namespace Hisubmit.Client.SharedModels.Features.StaticPages.Queries;

public class GetDetailStaticPageQuery
{
    public int Id { get; set; }
    public  string Link { get; set; }
    public  bool IsEnable { get; set; }
}


public class GetDetailStaticPageResponse
{
    public  int Id { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public string Content { get; set; }
    public bool IsEnable { get; set; }
    public FaqType FaqType { get; set; }
    public AddEditSeoTagRequest SeoTag { get; set; }
}
