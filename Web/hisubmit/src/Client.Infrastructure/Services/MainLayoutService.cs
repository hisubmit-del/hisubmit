using System;

namespace HiSubmit.Client.Infrastructure.Services;

public class MainLayoutService
{
    public event EventHandler<bool>? ChangedVisibleDrawer;
    public event EventHandler? ChangedSelectedAccount;
    public event EventHandler? UserLoginedAccount;

    public void ChangeDrawerStatus(bool showDrawer)
    {
        ChangedVisibleDrawer?.Invoke(this, showDrawer);
    }

    public void ChangeSelectedAccount()
    {
        ChangedSelectedAccount?.Invoke(this, EventArgs.Empty);
    }

    public void UserLoginAccount()
    {
        UserLoginedAccount?.Invoke(this, EventArgs.Empty);
    }
}
