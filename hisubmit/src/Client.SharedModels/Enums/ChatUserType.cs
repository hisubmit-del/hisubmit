using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums;

public enum ChatUserType
{
    [Display(Name = "Artist")]
    User=0,
    [Display(Name = "FestivalId")]
    Festival=1,
    [Display(Name = "Referee")]
    Referee=2,
    [Display(Name = "Admin")]
    Admin=3,
    [Display(Name = "SubUser")]
    FestivalSubUser=4
}