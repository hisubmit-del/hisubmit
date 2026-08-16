using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Domain.Enums;

public enum SoldTicketStatus:byte
{
    Default=0,
    [Display(Name = "Dont paid")]
    AwaitingPayment=1,
    [Display(Name = "Paid")]
    Paid=2,
    [Display(Name = "Canceled")]
    Canceled=3
}
