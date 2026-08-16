using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace HiSubmit.Domain.Enums;

public enum FeeStatus:int
{
    [Display(Name = "Default")]
    Usual,
    [Display(Name = "Special")]
    Special,
    [Display(Name = "Rejected")]
    Rejected,
    [Display(Name = "Special request ")]
    SpecialRequest 
}
