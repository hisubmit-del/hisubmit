using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Enums.Advertises;
using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Advertises.Commands;

public class AddAdvertiseRequest 
{
    public string Description { get; set; }
    public string Email { get; set; }
    public string Text { get; set; }
    public string UserId { get; set; }
    public AdvertiseType AdvertiseType { get; set; }
    public List<ImageDto> Images { get; set; }
    public List<AttachFileDto> Files { get; set; }

    public AddAdvertiseRequest()
    {
        Images = new List<ImageDto>();
        Files = new List<AttachFileDto>();
    }
}

public class ImageDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public int? FestivalId { get; set; }
    public int? AdvertiseRequestId { get; set; }
    public UploadRequest UploadRequest { get; set; }

    public ImageDto()
    {
        UploadRequest = new UploadRequest
        {
            UploadType = UploadType.Advertise
        };
    }
}

public class AttachFileDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public FileFormat FileFormat { get; set; }
    public UploadRequest UploadRequest { get; set; }
    public int AdvertiseRequestId { get; set; }

    public AttachFileDto()
    {
        UploadRequest = new UploadRequest()
        {
            UploadType = UploadType.Document,
        };
    }
}


