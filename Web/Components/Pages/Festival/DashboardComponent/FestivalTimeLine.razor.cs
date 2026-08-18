using System;
using MudBlazor;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLine;

namespace Web.Components.Pages.Festival.DashboardComponent;

public partial class FestivalTimeLine
{
    #region Parameters

    [Parameter] public int FestivalId { get; set; }
    [Parameter] public GetFestivalDetailResponse Festival { get; set; }

    #endregion

    #region Inject

    [Inject] public IPublicFestivalManager FestivalManager { get; set; }

    #endregion

    private List<GetAllDeadLineResponse> DeadLines { get; set; }

    public List<TimeLineItem> TimeLines { get; set; }

    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadDeadLines();
        await GenerateItems();
        _loaded = true;
    }

    private async Task LoadDeadLines()
    {
        var response = await FestivalManager.GetAllDeadlineEntry(new GetAllDeadlineQuery()
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
        {
            DeadLines = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task GenerateItems()
    {
        await Task.Run(() =>
        {
            var deadLines = new List<TimeLineItem>();
            deadLines.AddRange(DeadLines.Select(p => new TimeLineItem()
            {
                Date = p.Date,
                Name = p.Name
            }).ToList());
            if (Festival.OpeningDate != null)
            {
                deadLines.Add(new TimeLineItem()
                {
                    Name = localizer["Opening Date"],
                    Date = Festival.OpeningDate.Value
                });
            }

            if (Festival.NotificationDate != null)
                deadLines.Add(new TimeLineItem()
                {
                    Name = localizer["Notification Date"],
                    Date = Festival.NotificationDate.Value
                });


            if (Festival.EventEndDate != null)
                deadLines.Add(new TimeLineItem()
                {
                    Name = localizer["Event Start Date"],
                    Date = Festival.EventStartDate.Value
                });

            if (Festival.EventStartDate != null)
                deadLines.Add(new TimeLineItem()
                {
                    Name = localizer["Event End Date"],
                    Date = Festival.EventEndDate.Value
                });
            //get nextDate

            var nextDate = deadLines.SkipWhile(p => p.Date <= DateTime.Now).FirstOrDefault();
            if (nextDate != null)
            {
                nextDate.Nearest = true;
            }

            TimeLines = deadLines.OrderBy(p => p.Date).ToList();
        });
    }
}

public class TimeLineItem
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public bool Nearest { get; set; }
}
