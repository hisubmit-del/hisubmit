using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Queries.FestivalPaymentStates;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Client.Infrastructure.Managers.FestivalPayments;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using Severity = MudBlazor.Severity;

namespace ClientComponents.Pages.Festival.DashboardComponent;

public partial class PurchaseDunateChart
{
    #region Injects

    [Inject] private IFestivalPaymentsManager FestivalPaymentsManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public GetFestivalPaymentStateResponse State { get; set; }

    #endregion


    protected override async Task OnInitializedAsync()
    {
        // await LoadItems();

        GenerateSeries();
        _loaded = true;
        await base.OnInitializedAsync();
    }

    private List<GetCartItemResponse> _cartItems;
    private bool _loaded;
    private double[] _data = { 0, 0, 0 };
    private string[] _labels = { "Product", "Submit", "Tickets" };
    private string _showTotal;

    private string _showTitle;
    // private async Task LoadItems()
    // {
    //     var response = await FestivalPaymentsManager.GetAll(new GetAllCartItemQuery
    //     {
    //         GetAllData = true,
    //         FestivalId = FestivalId,
    //         ItemType = GetCartItemQueryType.FestivalId,
    //     });
    //     if (response.Succeeded)
    //         _cartItems = response.Data;
    //     else
    //         foreach (var message in response.Messages)
    //             _snackBar.Add(message, Severity.Error);
    // }

    private void GenerateSeries()
    {
        _data[0] = (double)State.Product;
        _data[1] = (double)State.Submit;
        _data[2] = (double)State.Ticket;
        _showTotal = _data.Sum().ToString("C2");
        _showTitle = "Total";
    }

    private void ShowTotalType(int index)
    {
        _showTitle = _labels[index];
        _showTotal = _data[index].ToString("C2");
    }

    private void ShowAllTotal()
    {
        _showTotal = _data.Sum().ToString("C2");
        _showTitle = "Total";
    }
}