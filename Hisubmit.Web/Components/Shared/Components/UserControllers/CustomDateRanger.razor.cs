using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Web.Components.Shared.Components.UserControllers;

public partial class CustomDateRanger
{
    private DateRange _dateRange = new DateRange();

    #region Parameters

    [Parameter] public string PlaceholderStart { get; set; }
    [Parameter] public string PlaceholderEnd { get; set; }
    [Parameter] public DateTime? StartDate { get; set; }

    [Parameter] public DateTime? EndDate { get; set; }

    #endregion

    protected override Task OnInitializedAsync()
    {
        _dateRange = new DateRange(StartDate, EndDate);
        return base.OnInitializedAsync();
    }

    private Task ChangeStartAndEndDate(DateRange dr)
    {
        EndDate = dr.End;
        StartDate = dr.Start;
        _dateRange = dr;
        return Task.CompletedTask;
    }
}