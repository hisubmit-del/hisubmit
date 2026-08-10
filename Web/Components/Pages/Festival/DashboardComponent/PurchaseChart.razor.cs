using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Client.Infrastructure.Constants;
using HiSubmit.Client.Infrastructure.Managers.FestivalPayments;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Pages.Festival.DashboardComponent;

public partial class PurchaseChart
{
    #region Injects

    [Inject] public IFestivalPaymentsManager FestivalPaymentsManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int FestivalId { get; set; }

    #endregion

    #region Private Field

    private bool _loaded;
    private int Tick = 1;
    private int MaxTick = 10;
    private string[] xLabels = { };
    private List<ChartSeries> Series = new();
    private List<GetCartItemResponse> _cartItems = new();

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadItems();
        GenerateSeries();
        _loaded = true;
        await base.OnInitializedAsync();
    }

    #endregion

    private async Task LoadItems()
    {
        var response = await FestivalPaymentsManager.GetAll(new GetAllCartItemQuery
        {
            GetAllData = true,
            FestivalId = FestivalId,
            Type = GetCartItemQueryType.Festival,
        });
        if (response.Succeeded)
            _cartItems = response.Data;
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
    }

    private void GenerateSeries()
    {
        var dailyPurchase = _cartItems.OrderBy(p=>p.PaidDate).GroupBy(p => p.PaidDate.Date).ToList();
        xLabels = new string[dailyPurchase.Count + 1];
        var series = new ChartSeries()
        {
            Name = "Daily Purchase (Submit)",
            Data = new double[dailyPurchase.Count + 1]
        };
        xLabels[0] = string.Empty;
        series.Data[0] = 0;
        for (var i = 1; i <= dailyPurchase.Count; i++)
        {
            xLabels[i] = (dailyPurchase[i - 1].Key.Date.ToString("MMM dd"));
            series.Data[i] = (double)dailyPurchase[i - 1].Sum(p => p.Price);
        }

        SetMaxTick((int)series.Data.Max());
        SetTick();
        Series.Add(series);
    }

    private void SetMaxTick(int maxValue)
    {
        do
        {
            maxValue++;
        } while (maxValue % 5 == 0);

        MaxTick = maxValue;
    }

    private void SetTick()
    {
        Tick = MaxTick / 10;
        if (Tick < 1)
            Tick = 1;
    }

    private ChartOptions _chartOptions => new()
    {
        YAxisLines = true,
        YAxisTicks = Tick,
        MaxNumYAxisTicks = 10,
    };
}