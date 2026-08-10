using System;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.SoldProducts.Commands;
using HiSubmit.Client.Infrastructure.Managers.ProductsSold;
using HiSubmit.Client.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Web.Components.Shared.Dialogs;

public partial class AddProductSold
{
    #region Injects

    [Inject] public UserCartService UserCartService { get; set; }
    [Inject] private IProductSoldManager ProductSoldManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public AddProductSoldCommand Product { get; set; }
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }

    #endregion

    #region Private Field

    private bool _processing;
    private FluentValidationValidator _fluentValidationValidator;

    #endregion

    protected override async Task OnInitializedAsync()
    {
        if (!(await AuthenticationManager.CurrentUser()).Identity.IsAuthenticated)
        {
            MudDialog.Close();
            await _dialogService.ShowAsync<NeedToLogin>("Need To Login");
        }

        await base.OnInitializedAsync();
    }

    private async Task SaveAsync()
    {
        var validated = _fluentValidationValidator
            .Validate(param => param.IncludeAllRuleSets());
        if (validated)
        {
            _processing = true;
            var response = await ProductSoldManager.AddAsync(Product);
            if (response.Succeeded)
            {
                _snackBar.Add(Localize["Product Add To Cart"], Severity.Success);
                UserCartService.ChangeUserCart();
                MudDialog.Close();
            }
            else
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }

            _processing = false;
        }
    }

    private void Cancel()
    {
        MudDialog.Close();
    }
}
