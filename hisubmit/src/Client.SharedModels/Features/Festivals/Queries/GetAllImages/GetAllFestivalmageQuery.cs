using Hisubmit.Client.SharedModels.Wrapper;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllImages;

public class GetAllFestivalImageQuery:PagedRequest
{
    public int FestivalId { get; set; }
}

public class GetAllFestivalImageResponse
{
    public  int Id { get; set; }
    public string Title { get; set; }
    public  string Url { get; set; }
    
    public  int FestivalId { get; set; }
    public ImageType ImageType { get; set; }
}

