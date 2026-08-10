using Hisubmit.Client.SharedModels.Features.Seo;

namespace Hisubmit.Client.SharedModels.Features.News.Queries;

public class GetDetailNewQuery
{
    public int Id { get; set; }
}


public class GetDetailNewResponse
{
    public  int Id { get; set; }
    public  string Title { get; set; }
    public  string BannerUrl { get; set; }
    public  string Description { get; set; }
    public  bool IsEnable { get; set; }
    public string ShortDescription { get; set; }
    
    public AddEditSeoTagRequest SeoTag { get; set; }

}