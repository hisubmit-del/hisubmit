using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums;

public enum ProductSoldStatus
{
    Default=0,
    [Display(Name = "Dont paid")]
    AwaitingPayment=1,
    [Display(Name = "Paid")]
    Paid=2,
    [Display(Name = "Canceled")]
    Canceled=3
}