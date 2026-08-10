using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Application.Enums;

public enum ChatUserType
{
    [Display(Name = "Artist")]
    User=0,
    [Display(Name = "Festival")]
    Festival=1,
    [Display(Name = "Referee")]
    Referee=2,
    [Display(Name = "Admin")]
    Admin=3,
    [Display(Name = "SubUser")]
    FestivalSubUser=4
}