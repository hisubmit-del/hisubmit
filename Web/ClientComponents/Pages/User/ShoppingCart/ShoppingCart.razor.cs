using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Payments.Commands;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Client.Infrastructure.Managers.Payments;
using HiSubmit.Client.Infrastructure.Services;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace ClientComponents.Pages.User.ShoppingCart;

public partial class ShoppingCart
{
    #region Injects
    [Inject] private ICartManager CartManager { get; set; }
    [Inject] public UserCartService UserCartService { get; set; }
    #endregion


    private List<GetCartItemResponse> _shopCartItems = new();
    private bool IsVisibleOveralls { get; set; }
    private List<string> _discountCodes = [];
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadCartItems();
        await LoadUserPaidItems();
    }

    private async Task LoadUserPaidItems()
    {
        var response = await CartManager.GetAll(new GetAllCartsFilterDto
        {
            Paid = true,
            GetAllData = true,
            TakeCurrentUserCarts = true,
        });
        if (response.Succeeded)
            foreach (var cart in response.Data)
                _shopCartItems.AddRange(cart.CartItems);
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);

    }

    private async Task DeleteItem(GetCartItemResponse item)
    {
        var response = await CartManager.DeleteItem(new DeleteCartItemCommand
        {
            Id = item.Id
        });
        if (response.Succeeded)
        {
            await LoadCartItems();
            await LoadUserPaidItems();
        }
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
    }

    private async Task LoadCartItems()
    {
        var response = await CartManager.GetItems(new GetUserOpenCartItemQuery());

        if (response.Succeeded)
            _shopCartItems = response.Data;
    }

    private async Task PaidCart()
    {
        var response = await CartManager.PaidCart(new PaidCartRequest());
        if (response.Succeeded)
        {
            _snackBar.Add("Successfully paid cart", Severity.Success);
            await LoadCartItems();
            StateHasChanged();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task PaidSuccessfully(IResult<CheckPaymentResponse> response)
    {
        _snackBar.Add("Successfully paid cart", Severity.Success);
        UserCartService.ChangeUserCart();
        _navigationManager.NavigateTo($"user/payment-result/{response.Data.PaymentId}");
        // await LoadCartItems();
        // await LoadUserPaidItems();
        // StateHasChanged();
    }

    private async Task PaidZero()
    {
        var response = await CartManager.PaidZero(new PaidZeroCartRequest());
        if (response.Succeeded)
        {
            _snackBar.Add("Successfully paid cart", Severity.Success);
            await LoadCartItems();
            StateHasChanged();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private string _discountCode = string.Empty;
    private bool _calculateDiscountProcessing;
    private async Task AddDiscountCodes()
    {
        if(string.IsNullOrWhiteSpace(_discountCode))
            return;
        _discountCodes.Add(_discountCode);
        await CalculateDiscountCodes();
    }
    private async Task RemoveDiscountCode(string s)
    {
        _discountCodes.Remove(s);
        await CalculateDiscountCodes();
    }

    private async Task CalculateDiscountCodes()
    {
        _calculateDiscountProcessing = true;
        var res = await CartManager.CalculateDiscountCodes(new CalculateDiscountCodesRequest
        {
            DiscountCodes = _discountCodes,
            CartId = _shopCartItems.Where(p => !p.Paid).Select(p => p.CartId).FirstOrDefault()
        });
        if (res.Succeeded)
        {
            foreach (var p in _shopCartItems.Where(p => !p.Paid))
            {
                var f = res.Data.FirstOrDefault(x => x.Id == p.Id);
                if (f!=null)
                    p.PriceAfterDiscount = f.PriceAfterDiscount;
            }
            //     _shopCartItems = res.Data.ToList();
            //   _snackBar.Add("Discount codes applied successfully", Severity.Success);
            _discountCode = string.Empty;
            StateHasChanged();
        }
        else
            foreach (var message in res.Messages)
                _snackBar.Add(message, Severity.Error);
        _calculateDiscountProcessing = false;
    }
}