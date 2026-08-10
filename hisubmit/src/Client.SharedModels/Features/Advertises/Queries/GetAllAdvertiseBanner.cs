using Hisubmit.Client.SharedModels.Wrapper;
using Hisubmit.Client.SharedModels.Enums;


namespace Hisubmit.Client.SharedModels.Features.Advertises.Queries;

public class GetAllAdvertiseBannerRequest:
    PagedRequest{
    public string SearchString { get; set; }
    public  bool? IsOpen { get; set; }
}


public class GetAllAdvertiseBannerResponse
{
    public int Id { get; set; }
    public  string Url { get; set; }
    public  string Title { get; set; }
    public  DateTime OpenDateTime { get; set; }
    public  DateTime CloseDateTime { get; set; }
    public  AdvertiseBannerPosition Position { get; set; }   
}