using System;


namespace HiSubmit.Client.Infrastructure.Services;

public class UserCartService
{
    public event EventHandler? UserCartChanged;

    public void ChangeUserCart()
    {
        UserCartChanged?.Invoke(this,EventArgs.Empty);
    }
}