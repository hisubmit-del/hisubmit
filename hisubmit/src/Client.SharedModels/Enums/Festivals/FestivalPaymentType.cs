using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums.Festivals;

public enum FestivalPaymentType:int
{
    [Display(Name = "Not selected")]
    NotSelected=0,
    [Display(Name = "Paypal")]
    Paypal=2,
    [Display(Name = "Debit card")]
    DebitCard=1,
}