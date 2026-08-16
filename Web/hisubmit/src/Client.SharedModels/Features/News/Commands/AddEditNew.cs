using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.News.Commands;

public class AddEditNewCommand 
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string BannerUrl { get; set; }

    public string Description { get; set; }

    // public bool IsEnable { get; set; }
    public string ShortDescription { get; set; }

    public string ImageALt { get; set; }
    public int? FestivalId { get; set; }
    public UploadRequest UploadRequest { get; set; } = new();
    public AddEditSeoTagRequest SeoTag { get; set; } = new();
}
