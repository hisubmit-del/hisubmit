using Hisubmit.Client.SharedModels.Features.Advertises.Commands;
using Hisubmit.Client.SharedModels.Enums.Advertises;

namespace Hisubmit.Client.SharedModels.Features.Advertises.Queries;

public class GetDetailAdvertiseRequest 
{
    public int Id { get; set; }
}


public class GetDetailAdvertiseResponse
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Description { get; set; }
    public List<ImageDto> Images { get; set; } = new();
    public List<AttachFileDto> Files { get; set; } = new();
    public AdvertiseType AdvertiseType { get; set; }
}
