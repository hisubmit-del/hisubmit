using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditShowHall;
using HiSubmit.Client.Infrastructure.Managers.Venues;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClientComponents.Pages.Festival.Tickets.Venue;

public partial class AddEditShowHall
{
    [Inject] private IVenueManager VenueManager { get; set; }

    [Parameter] public AddEditShowHallCommand ShowHall { get; set; } = new();

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }

    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated { get; set; } = true;
    private  List<AddEditShowingDay> ShowingTimes { get; set; }

    private bool _processing;

    protected override async Task OnInitializedAsync()
    {
         GenerateShowingTimesBox();
        await base.OnInitializedAsync();
    }

    private async Task SaveAsync()
    {
        _processing = true;
        ShowHall.ShowTimes.Clear();
        foreach (var showtime in ShowingTimes)
        {
            foreach (var time in showtime.ShowingTimes)
            {
                ShowHall.ShowTimes.Add(new ShowTimeDto()
                {
                    ShowHallId = ShowHall.Id,
                    Id = time.Id,
                    OpenDate = showtime.DateTime?.Add(time.OpenDate ?? new TimeSpan()),
                    CloseDate = showtime.DateTime?.Add(time.CloseDate ?? new TimeSpan()),
                    Name = time.Name
                });
            }
        }
        
        
        Validated = _fluentValidationValidator.Validate(action => action.IncludeAllRuleSets());
      
        if (Validated)
        {
            var response = await VenueManager.SaveShowHall(ShowHall);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
        _processing = false;
    }

    private void Cancel()
    {
        MudDialog.Close();
    }

    private void AddShowingDay()
    {
        ShowingTimes.Add(new AddEditShowingDay()
        {
            ShowingTimes = new List<AddEditShowingTime>(){new AddEditShowingTime()}
        });
    }

    private void AddShowing(AddEditShowingDay day)
    {
        day.ShowingTimes.Add(new AddEditShowingTime());
    }

    private void DeleteDay(AddEditShowingDay day)
    {
        ShowingTimes.Remove(day);
    }

    private void DeleteTime(AddEditShowingDay day,AddEditShowingTime time)
    {
        day.ShowingTimes.Remove(time);
    }

    private void GenerateShowingTimesBox()
    {
        var listOfTimeBox = new List<AddEditShowingDay>();
        foreach (var showTime in ShowHall
                     .ShowTimes.GroupBy(p => p.OpenDate?.Date))
        {
            var timesShowing = showTime.Select(p => new AddEditShowingTime()
            {
                Name = p.Name,
                Id = p.Id,
                OpenDate = p.OpenDate?.TimeOfDay,
                CloseDate = p.CloseDate?.TimeOfDay
            }).ToList();
            listOfTimeBox.Add(new AddEditShowingDay()
            {
                ShowHallId = ShowHall.Id,
                DateTime = showTime.Key?.Date,
                ShowingTimes = timesShowing,
            });
        }

        ShowingTimes = listOfTimeBox;
    }
}

public class AddEditShowingDay
{
    public  int ShowHallId { get; set; }
    public DateTime? DateTime { get; set; }
    public List<AddEditShowingTime> ShowingTimes { get; set; }

    public AddEditShowingDay()
    {
        ShowingTimes = new List<AddEditShowingTime>();
    }
}

public class AddEditShowingTime
{
    public int Id { get; set; }

    public  string Name { get; set; }
    public TimeSpan? OpenDate { get; set; }
    public TimeSpan? CloseDate { get; set; }
}