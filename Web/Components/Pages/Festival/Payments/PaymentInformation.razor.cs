using System.Threading.Tasks;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Queries.GetDetail;
using HiSubmit.Client.Infrastructure.Managers.AdminPaymentManager;
using HiSubmit.Client.Infrastructure.Managers.FestivalPayments;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Pages.Festival.Payments;

public partial class PaymentInformation
{
    #region Inject

    [Inject] private IMapper Mapper { get; set; }
    [Inject] private IFestivalPaymentsManager PaymentManager { get; set; }

    #endregion

    #region Private Field

    private AddEditFestivalPaymentInformationCommand _information = new();
    private FluentValidationValidator _fluentValidationValidator;
    private bool _processing;
    private bool _readOnly = true;
    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await base.CheckPermission(Permissions.FestivalPayment.PaymentInformation);
        await LoadInformation();
        _information.FestivalId = SelectedFestivalId;
        await base.OnInitializedAsync();
    }

    #endregion

    private async Task LoadInformation()
    {
        var response = await PaymentManager.GetFestivalPaymentInformationAsync
        (new GetFestivalPaymentInformationDetailQuery
        {
            FestivalId = SelectedFestivalId
        });
        if (response.Succeeded)
        {
            if (response.Data != null)
            {
            _information = Mapper.Map<AddEditFestivalPaymentInformationCommand>(response.Data);
            }
        }
    }

    private async Task SaveAsync()
    {
        var validate = _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (validate)
        {
        _processing = true;
            var response = await PaymentManager.UpdateFestivalPaymentInformation(_information);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0],Severity.Success);
                StateHasChanged();
            }

            _processing = false;
        }
    }
}