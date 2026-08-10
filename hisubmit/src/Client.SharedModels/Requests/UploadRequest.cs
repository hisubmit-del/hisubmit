using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Requests;

public class UploadRequest
{
    public string FileName { get; set; }
    public string Extension { get; set; }
    public UploadType UploadType { get; set; }
    public byte[] Data { get; set; }

    public string ImageId { get; set; }

    public UploadRequest()
    {
        UploadType = UploadType.Product;
        Data = Array.Empty<byte>();
        ImageId = Guid.NewGuid().ToString();
    }
}