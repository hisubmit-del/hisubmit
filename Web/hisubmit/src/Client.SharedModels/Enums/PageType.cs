using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums;


public enum PageType:byte
{
    HomePage = 0,
    News = 1,
    FestivalPage = 2,
    StaticPage = 3,
    NewsList = 4,
    Product = 5,
    [Display(Name = "F&Q")]
    FAQ = 6,
    [Display(Name = "Advertise Form Page")]
    Advertise = 7
}
