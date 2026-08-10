using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums;

public enum AdvertiseBannerPosition
{
    [Display(Name = "Side Bar Festival Pages")]
    SideBarFestival,

    [Display(Name = "Side Bar Other Pages")]
    SideBarOther,
}