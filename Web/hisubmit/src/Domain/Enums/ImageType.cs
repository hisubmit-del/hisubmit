using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Domain.Enums;

public enum ImageType:byte
{
    [Display(Name = "Gallery Images")]
    Images=0,
    [Display(Name = "Cover")]
    Cover=1
}
