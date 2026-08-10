using HiSubmit.Domain.Entities.Festivals;
using MediatR;

namespace HiSubmit.Application.Events.FestivalChangeDeadLine;

public class FestivalChangeDeadlineEvent:INotification
{
    public  Festival Festival { get; set; }
}