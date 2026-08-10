using System;

namespace HiSubmit.Client.Infrastructure.Services;

public class UserNotificationService
{
    public event EventHandler? NotificationSeen;

    public void ChangeNotificationBar()
    {
        NotificationSeen?.Invoke(this,EventArgs.Empty);
    }
}