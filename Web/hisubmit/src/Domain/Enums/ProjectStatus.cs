using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Domain.Enums;

public enum ProjectStatus:byte
{
    [Display(Name = "Default")]
    Default=1,
    [Display(Name = "Released")]
    Released=2
}
