using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLine;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using HiSubmit.Client.Pages.Festival;
using HiSubmit.Client.Pages.Project.Specification;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Client.Pages.Public.Festivals.Components;

public partial class FestivalTimeLine
{
    [Parameter] public int FestivalId { get; set; }
    [Parameter] public GetFestivalDetailResponse Festival { get; set; }
[Parameter] public EventCallback SubmitClicked { get; set; }
    private List<GetAllDeadLineResponse> DeadLines { get; set; } = new();

    public List<TimeLineItem> TimeLines { get; set; } = new();

    [Inject] public IPublicFestivalManager FestivalManager { get; set; }
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        _subscription = ApplicationState.RegisterOnPersisting(PersistFestival);
        await base.OnInitializedAsync();
        await LoadDeadLines();
        await GenerateItems();
        _loaded = true;
    }

    private async Task LoadDeadLines()
    {
        if (ApplicationState.TryTakeFromJson<List<GetAllDeadLineResponse>>
                ("deadLine", out var stored))
        {
            DeadLines = stored;
        }
        else
        {
            var response = await FestivalManager.GetAllDeadlineEntry(new GetAllDeadlineQuery()
            {
                FestivalId = FestivalId
            });
            if (response.Succeeded)
                DeadLines = response.Data;
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, MudBlazor.Severity.Error);
        }
    }

    private async Task GenerateItems()
    {
        if (ApplicationState.TryTakeFromJson<List<TimeLineItem>>("timeLine", out var stored))
        {
            TimeLines = stored;
        }
        else
        {
            if (DeadLines.Any())
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
                        deadLines.Add(new TimeLineItem()
                        {
                            Name = Localize["Opening Date"],
                            Date = Festival.OpeningDate.Value
                        });

                    if (Festival.NotificationDate != null)
                        deadLines.Add(new TimeLineItem()
                        {
                            Name = Localize["Notification Date"],
                            Date = Festival.NotificationDate.Value
                        });

                    if (Festival.EventStartDate != null)
                        deadLines.Add(new TimeLineItem()
                        {
                            Name = Localize["Event Date"],
                            Date = Festival.EventStartDate.Value
                        });

                    if (Festival.EventEndDate != null)
                        deadLines.Add(new TimeLineItem()
                        {
                            Name = Localize["Event End Date"],
                            Date = Festival.EventEndDate.Value
                        });
                    //get nextDate

                    var nextDate = deadLines.OrderBy(p => p.Date)
                        .SkipWhile(p => p.Date.Date < DateTime.Now.Date)
                        .MinBy(p => p.Date.Date);

                    if (nextDate != null)
                    {
                        nextDate.Nearest = true;
                    }

                    TimeLines = deadLines.OrderBy(p => p.Date).ToList();
                });
            }
        }
    }

    private async Task Submit()
    {
        await SubmitClicked.InvokeAsync();
    }

    public DateTime? GetLastDeadLine()
    {
        return DeadLines.OrderBy(p => p.Date)
            .Select(p=>p.Date).LastOrDefault();
    }

    #region Prerendering

    private PersistingComponentStateSubscription _subscription;

    private Task PersistFestival()
    {
        ApplicationState.PersistAsJson("deadLine", DeadLines);
        ApplicationState.PersistAsJson("timeLine", TimeLines);
        return Task.CompletedTask;
    }

    #endregion
}

public class TimeLineItem
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public bool Nearest { get; set; }
}