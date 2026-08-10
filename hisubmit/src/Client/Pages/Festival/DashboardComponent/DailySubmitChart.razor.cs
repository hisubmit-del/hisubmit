using System;
using MudBlazor;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;

namespace HiSubmit.Client.Pages.Festival.DashboardComponent;

public partial class DailySubmitChart
{
    [Parameter] public List<GetAllSubmitsResponse> Submits { get; set; }
    [Parameter] public bool Loaded { get; set; }

    private int _tick = 1;
    private int _maxTick = 5;
    private string[] _xLabels = { };
    private List<ChartSeries> _series = new();

    protected override async Task OnInitializedAsync()
    {
        if (Submits.Any())
        {
            GenerateSeries();
        }

        await base.OnInitializedAsync();
    }

    private void GenerateSeries()
    {
        var dailyGroupedSubmits = Submits.GroupBy(p => p.SubmitDate.Date).ToList();

        _xLabels = new string[dailyGroupedSubmits.Count + 1];
        var ser = new ChartSeries()
        {
            Name = "Daily Submit",
            Data = new double[dailyGroupedSubmits.Count + 1]
        };
        _xLabels[0] = string.Empty;
        ser.Data[0] = 0;
        for (var i = 1; i <= dailyGroupedSubmits.Count; i++)
        {
            _xLabels[i] = (dailyGroupedSubmits[i - 1].Key.Date.ToString("MMM dd"));
            ser.Data[i] = dailyGroupedSubmits[i - 1].Count();
        }

        _maxTick = (int)ser.Data.Max();
        SetTick();
        _series.Add(ser);
    }

    private void SetTick()
    {
        _tick = _maxTick / 5;
        if (_tick < 1)
            _tick = 1;
    }
}