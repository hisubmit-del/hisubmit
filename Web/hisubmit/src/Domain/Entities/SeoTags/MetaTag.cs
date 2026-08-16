using System.ComponentModel.DataAnnotations;
using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.SeoTags;

public class MetaTag:AuditableEntity<int>
{
    public  string  PageId { get; set; }
    public string PageTitle { get; set; }
    public PageType Type { get; set; }
    
    //Tags
    public string Title { get; set; }
    public string Description { get; set; }
    public bool  Index { get; set; }
    public bool Follow { get; set; }

    public string CanonicalUrl { get; set; }
    public string MetaKeywords { get; set; }


    //public string OgImageUrl { get; set; }


}

public enum PageType:byte
{
    HomePage=0,
    News=1,
    FestivalPage=2,
    StaticPage=3,
    NewsList=4,
    Product=5,
    [Display(Name = "F&Q")]
    FAQ=6,
    [Display(Name = "Advertise Form Page")]
    Advertise = 7
}