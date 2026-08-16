using System;

namespace HiSubmit.Client.Infrastructure.Services;

public class SelectedAccountService
{
    public event EventHandler<int?>? SelectedAccount;

    public void SelectedAccountChanged(int? festivalId)
    {
        SelectedAccount?.Invoke(this,festivalId);
    }

    public event EventHandler<int> AdminLoginToFestival;
    
    public void AdminLoginedToFestival(int festivalId)
    {
        AdminLoginToFestival.Invoke(this, festivalId);
    }
}
