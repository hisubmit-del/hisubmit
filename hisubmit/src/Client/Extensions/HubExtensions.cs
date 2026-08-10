using System;
using HiSubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace HiSubmit.Client.Extensions;

public static class HubExtensions
{
    public static HubConnection TryInitialize(this HubConnection hubConnection, NavigationManager navigationManager)
    {
        return hubConnection ??= new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri(ApplicationConstants.SignalR.HubUrl))
            .Build();
    }
}