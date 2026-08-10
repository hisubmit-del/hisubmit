using MediatR;

namespace HiSubmit.Application.Events.Festivals.ViolationReportField;

public class ViolationReportFieldEvent:INotification
{
    public  string UserId { get; set; }
    public  int FestivalId { get; set; }
}