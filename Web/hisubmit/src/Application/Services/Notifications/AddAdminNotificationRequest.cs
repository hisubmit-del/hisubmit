using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Services;

public class AddAdminNotificationRequest
{
    public string Title { get; set; }
    public string Link { get; set; }
    public NotificationType NotificationType { get; set; }
}