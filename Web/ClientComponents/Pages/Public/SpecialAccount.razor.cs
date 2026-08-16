using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using Hisubmit.Client.SharedModels.Features.Users.Commands.SpecialFee;
using HiSubmit.Client.Infrastructure.Managers.Contents;
using HiSubmit.Client.Infrastructure.Managers.Payments;
using HiSubmit.Client.Infrastructure.Services;
using ClientComponents.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClientComponents.Pages.Public;

public partial class SpecialAccount
{
    #region Inject

    [Inject] public ICartManager CartManager { get; set; }
    [Inject] private IContentManager ContentManager { get; set; }
    [Inject] public UserCartService UserCartService { get; set; }

    #endregion

    private GetSiteCommissionResponse _commission = new();


    private bool _processing;
    private List<GetAllNewResponse> _news = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadSiteCommission();
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadNews();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadNews()
    {
        var response = await ContentManager.GetAllNew(new GetAllNewRequest()
        {
            PageSize = 5
        });
        if (response.Succeeded)
        {
            _news = response.Data;
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

    private async Task LoadSiteCommission()
    {
        var response = await CartManager.GetSpecialAccountFee();
        if (response.Succeeded)
        {
            _commission = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task AddToCard(StatusFeePeriod period)
    {
        var user = await AuthenticationManager.CurrentUser();
        if (!user.Identity.IsAuthenticated)
        {
            var option = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                
            };
            var parameters = new DialogParameters();
            var d = _dialogService.Show<NeedToLogin>("Need To Login", parameters, option);
            var r = await d.Result;
            return;
        }

        _processing = true;
        var response = await CartManager.AddSpecialAccountToCard(new SpecialFeeCommand()
        {
            Period = period
        });
        if (response.Succeeded)
        {
            _snackBar.Add(response.Messages[0], Severity.Success);
            UserCartService.ChangeUserCart();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        _processing = false;
    }
}