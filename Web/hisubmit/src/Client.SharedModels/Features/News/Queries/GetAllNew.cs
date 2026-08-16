using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.News.Queries;

public class GetAllNewRequest:PagedRequest
{
    public  bool? IsEnable { get; set; }
    public  int? FestivalId { get; set; }
    public string SearchString { get; set; }
    public bool ReturnLastNews { get; set; }
    public  bool GetFestivalNews { get; set; }

}

public class GetAllNewResponse
{
    public int Id { get; set; }
    public  string Title { get; set; }
    public  string BannerUrl { get; set; }
    public string ShortDescription { get; set; }

    public  int? FestivalId { get; set; }
    public string FestivalName { get; set; }
    public string FestivalLogoURL { get; set; }
    public bool IsEnable { get; set; }
    public  DateTime CreatedOn { get; set; }
    public  int CommentCount { get; set; }
    public bool  IsPined { get; set; }
}