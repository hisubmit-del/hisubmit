using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Domain.Enums;



public enum RefereeStatus
{
    [Display(Name = "Default")]
    Default=0,
    [Display(Name = "Remove From ProductFestivalId")]
    RemoveFromFestival=1,
    [Display(Name = "Remove From Project")]
    RemoveFromProject=2
}