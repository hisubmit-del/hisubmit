
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Advertises.Commands;

public class AddEditAdvertiseBannerRequest 
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public DateTime? OpenDateTime { get; set; }
    public DateTime? CloseDateTime { get; set; }
    public UploadRequest UploadRequest { get; set; }
    public AdvertiseBannerPosition Position { get; set; }

    public AddEditAdvertiseBannerRequest()
    {
        UploadRequest = new UploadRequest();
    }
}


