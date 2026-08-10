using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Payments.Commands;
using HiSubmit.Client.Infrastructure.Managers.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace ClientComponents.Shared.Components.Payments;

public partial class PaypalButtons
{
    #region Injects

    [Inject] private ICartManager CartManager { get; set; }

    #endregion

    #region Parameters

    [Parameter]
    public string ClientId { get; set; } =
        "ASrg5BdD5zjbbOVTvFW9QG2brIt1zIoChtDZkg9pQNLB-ud89_rJa_B9TvbMumi75IK1le73P5AMEyuS";

    [Parameter] public string ContainerId { get; set; } = "paypal-button-container";


    [Parameter]
    public decimal Amount
    {
        get;
        set;
    }
    [Parameter] public string CostumeId { get; set; }
    [Parameter] public EventCallback<IResult<CheckPaymentResponse>> PaidSuccessfully { get; set; }

    [Parameter] public bool Overaly { get; set; }
    [Parameter] public EventCallback<bool> OveralyChanged { get; set; }


    [Parameter]public List<string> DiscountCodes { get; set; }
    #endregion

    #region private Field
    private decimal _previousAmount;
    private bool IsLoading { get; set; } = true;
    private DotNetObjectReference<PaypalButtons> _ref;

    private RenderFragment PaypalButtonSdk
    {
        get
        {
            RenderFragment form = b =>
            {
                b.OpenElement(0, "script");
                b.AddAttribute(0, "src", "https://www.paypal.com/sdk/js" +
                                         $"?client-id={ClientId}" + "&components=buttons"
                );
                b.AddAttribute(0, "data-sdk-integration-source", "button-factory");
                b.CloseElement();
            };
            return form;
        }
    }

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await SetDotNetReference();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!IsLoading) 
        {
            if (_previousAmount != Amount )
            {
                await _jsRuntime.InvokeVoidAsync("RemoveButtonContainer", ContainerId);
                IsLoading = true;
                await RenderButton();
                IsLoading = false;
                _previousAmount = Amount;
            }
        }

        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (IsLoading && firstRender)
        {
            //IsLoading = false;
            await RenderButton();
            IsLoading = false;
            _previousAmount = Amount;
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    #endregion

    private async Task RenderButton()
    {
        var res = false;
        while (!res)
        {
            await Task.Delay(4000);
            object[] parameters = { $"#{ContainerId}", "OnApprovedMethod", Amount, CostumeId };
            res = await _jsRuntime.InvokeAsync<bool>("RenderPaypalButton", parameters);
        }
    }

   

    [JSInvokable("OnApprovedMethod")]
    public async Task OnApproved(PaidCartRequest request)
    {
        Overaly = true;
        await OveralyChanged.InvokeAsync(Overaly);
        request.DiscountCodes = DiscountCodes;
       
        var response = await CartManager.CheckAndPaidCart(request);

        if (response.Succeeded)
            await PaidSuccessfully.InvokeAsync(response);
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        Overaly = false;
        await OveralyChanged.InvokeAsync(Overaly);
    }

    private async Task SetDotNetReference()
    {
        _ref = DotNetObjectReference.Create(this);
        await _jsRuntime.InvokeVoidAsync("GLOBAL.SetDotnetReferencePaypal", _ref);
    }
}