using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Queries.FestivalPaymentStates;
using HiSubmit.Client.Infrastructure.Managers.FestivalPayments;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Pages.Festival.Payments.Components;

public partial class FestivalPaymentState
{
    #region  Inject

    [Inject]
    private  IFestivalPaymentsManager PaymentsManager { get; set; }

    #endregion

    #region  Parameters

    [Parameter]
    public  int FestivalId { get; set; }

    #endregion

    #region Private Field

    private GetFestivalPaymentStateResponse _state=new ();
    private bool _loaded;

    #endregion

    protected override async Task OnInitializedAsync()
    {
        await LoadFestivalIncome();
        _loaded = true;
        await base.OnInitializedAsync();
    }

    public async Task LoadFestivalIncome()
    {
        var response = await PaymentsManager.GetFestivalPaymentState(new GetFestivalPaymentStateQuery
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
            _state = response.Data;
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);

        StateHasChanged();
    }
}