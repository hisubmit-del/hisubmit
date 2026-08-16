using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Notifications.Commands;

public class SeenNotificationCommand 
{
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    public SiteAccountType AccountType { get; set; }
    public NotificationType NotificationTypes { get; set; }
}
