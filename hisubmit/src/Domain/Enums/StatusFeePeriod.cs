using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Domain.Enums;

public enum StatusFeePeriod
{
    [Display(Name = "Monthly")]
    Monthly,
    [Display(Name = "Three Month")]
    ThreeMonth,
    [Display(Name = "Yearly")]
    Yearly
}