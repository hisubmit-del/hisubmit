using AutoMapper;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using HiSubmit.Client.Infrastructure.Managers.AdminFestival;
using HiSubmit.Client.Infrastructure.Managers.AdminPaymentManager;
using HiSubmit.Client.Pages.Festival.Payments.Components;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Queries.GetDetail;
using HiSubmit.Client.Shared.Components.Payments;
using MudBlazor;

namespace HiSubmit.Client.Pages.Admin.Festivals;

public partial class PaymentDetail
{
    #region Inject

    [Inject] private IMapper Mapper { get; set; }
    [Inject] private IAdminPaymentManager PaymentManager { get; set; }
    [Inject] private IAdminFestivalManager AdminFestivalManager { get; set; }

    #endregion

    [Parameter] public int FestivalId { get; set; }

    private FestivalPaymentState _festivalPaymentState;

    #region Private Field

    private GetFestivalPaymentInformationDetailResponse _information = new();
    private GetFestivalDetailResponse _detail = new();
    private FluentValidationValidator _fluentValidationValidator;
    private bool _processing;
    private bool _readOnly = true;
    private bool _loaded;
    private bool _paymentInformationProcessing;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadDetail();
        _loaded = true;
        await LoadInformation();
        _information.FestivalId = FestivalId;
        await base.OnInitializedAsync();
    }

    #endregion

    private async Task LoadDetail()
    {
        var response = await AdminFestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery
        {
            FestivalId = FestivalId
        });

        if (response.Succeeded)
            _detail = response.Data;
    }

    private async Task LoadInformation()
    {
        var response = await PaymentManager.GetFestivalPaymentInformationPaymentAsync
        (new GetFestivalPaymentInformationDetailQuery
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
        {
            if (response.Data != null)
            {
                _information = response.Data;
            }
        }
    }

    private async Task ShowPaymentInformation()
    {
        _paymentInformationProcessing = true;
        var info = await PaymentManager.GetFestivalPaymentInformationPaymentAsync(
            new GetFestivalPaymentInformationDetailQuery
            {
                FestivalId = FestivalId
            });
        _paymentInformationProcessing = false;
        if (info.Succeeded)
        {
            var parameters = new DialogParameters
            {
                { nameof(FestivalPaymentInformationModal.Information), info.Data }
            };
            var options = new DialogOptions
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
                
            };
            _dialogService.Show(typeof(FestivalPaymentInformationModal), "Payment Information", parameters, options);
        }
    }

    private async Task ReloadData()
    {
        await _festivalPaymentState.LoadFestivalIncome();
    }
}