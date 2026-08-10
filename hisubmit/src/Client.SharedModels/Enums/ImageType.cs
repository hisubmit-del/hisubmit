using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums;

public enum ImageType:byte
{
    [Display(Name = "Gallery Images")]
    Images=0,
    [Display(Name = "Cover")]
    Cover=1,
    [Display(Name = "Product")]
    Product=2
}
