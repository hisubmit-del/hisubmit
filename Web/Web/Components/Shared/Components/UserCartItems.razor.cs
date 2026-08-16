using System;
using Hisubmit.Client.SharedModels.Features.Payments.Commands;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Client.Infrastructure.Managers.Payments;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Services;
using MudBlazor;

namespace Web.Components.Shared.Components;

public partial class UserCartItems
{
    #region Inject

    [Inject] public ICartManager CartManager { get; set; }
    [Inject] public UserCartService UserCartService { get; set; }

    #endregion

    #region Private Field

    private bool _isOpen;

    private List<GetCartItemResponse> Items { get; set; } = new();

    #endregion

    protected override async Task OnInitializedAsync()
    {
        await LoadCartItems();
        UserCartService.UserCartChanged += async (s, h) => await OnChangeUserCartItem(s, h);
        await base.OnInitializedAsync();
    }


    private async Task LoadCartItems()
    {
        var response = await CartManager.GetItems(new GetUserOpenCartItemQuery()
        {
            UserId = string.Empty
        });
        // _snackBar.Add(response.Data.Count.ToString(), MudBlazor.Severity.Error);

        if (response.Succeeded)
        {
            Items = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }


    private async Task OnChangeUserCartItem(object? sender, EventArgs e)
    {
        await LoadCartItems();
        StateHasChanged();
    }

    private async Task PaidCart()
    {
        var response = await CartManager.PaidCart(new PaidCartRequest());
        if (response.Succeeded)
        {
            _snackBar.Add("Successfully paid cart", MudBlazor.Severity.Success);
            await LoadCartItems();
            StateHasChanged();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }


    private async Task DeleteItem(GetCartItemResponse item)
    {
        var response = await CartManager.DeleteItem(new DeleteCartItemCommand
        {
            Id = item.Id
        });
        if (response.Succeeded)
            await LoadCartItems();
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
    }

    private Task ShowCart()
    {
        _isOpen = !_isOpen;
        return Task.CompletedTask;
    }
}