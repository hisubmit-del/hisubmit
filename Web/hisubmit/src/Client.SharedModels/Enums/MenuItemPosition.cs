using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums;

public enum MenuItemPosition:byte
{
    [Display(Name = "Footer")]
    Footer=0,
    [Display(Name = "Header")]
    Header=1
}