using MediatR;

namespace HiSubmit.Application.Events.Payments.PaidCartEvent;

public class CartPaidedEvent:INotification
{
    public int CartId { get; set; }
}