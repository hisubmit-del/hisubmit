using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalImages;

public class AddEditFestivalImageCommand
{
    public  int FestivalId { get; set; }
    public List<FestivalImageDto> Images { get; set; } = new();
}



public class FestivalImageDto
{
    public int Id { get; set; }
    public int FestivalId { get; set; }
    public  string Title { get; set; }
    public  ImageType ImageType { get; set; }
    public  string Url { get; set; }
    public  UploadRequest UploadRequest { get; set; } = new();
}