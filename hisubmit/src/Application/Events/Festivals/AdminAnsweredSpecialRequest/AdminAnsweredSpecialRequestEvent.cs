using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Festivals.AdminAnsweredSpecialRequest;

public class AdminAnsweredSpecialRequestEvent:INotification
{
    public  int FestivalId { get; set; }
    public  FeeStatus FeeStatus { get; set; }
}