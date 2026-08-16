using MediatR;

namespace HiSubmit.Application.Events.Submits.PaidSubmit;

public class PaidSubmitEvent:INotification
{
    public int SubmitId { get; set; }
}

