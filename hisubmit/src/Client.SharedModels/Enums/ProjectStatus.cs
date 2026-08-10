using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums;

public enum ProjectStatus:byte
{
    [Display(Name = "Default")]
    Default=1,
    [Display(Name = "Released")]
    Released=2
}
