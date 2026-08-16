using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Services;

public class AddFestivalNotificationRequest
{
    public int FestivalId { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public NotificationType NotificationType { get; set; }
}