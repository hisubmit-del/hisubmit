using MediatR;

namespace HiSubmit.Application.Events.Payments.DeleteFestivalSubmitItemInCartItem;

public class DeletedFestivalSubmitItemInCartItemEvent : INotification
{
    public int  SubmitId { get; set; }
}