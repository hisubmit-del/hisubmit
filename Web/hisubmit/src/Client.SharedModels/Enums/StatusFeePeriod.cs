using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums;

public enum StatusFeePeriod
{
    [Display(Name = "Monthly")]
    Monthly,
    [Display(Name = "Three Month")]
    ThreeMonth,
    [Display(Name = "Yearly")]
    Yearly
}