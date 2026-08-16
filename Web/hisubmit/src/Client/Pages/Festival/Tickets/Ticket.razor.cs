using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetVenueById;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;
using Hisubmit.Client.SharedModels.Features.Tickets.Commands.AddEditTickets;
using Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetTicketById;
using HiSubmit.Client.Infrastructure.Managers.Tickets;
using HiSubmit.Client.Infrastructure.Managers.Venues;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.Constants.Storage;
using HiSubmit.Client.SharedModels.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using HiSubmit.Client.Shared.Dialogs;

namespace HiSubmit.Client.Pages.Festival.Tickets;

public partial class Ticket
{
    #region Inject

    [Inject] private IMapper Mapper { get; set; }
    [Inject] private IVenueManager VenueManager { get; set; }
    [Inject] private ITicketManager TicketManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int TicketId { get; set; }

    #endregion


    private bool _processing;
    private bool _loaded;
    private AddEditTicketsCommand _ticket = new();
    private List<GetAllVenueResponse> _venues = new();
    private GetVenueByIdResponse _selectedVenue = new();
    private FluentValidationValidator _fluentValidationValidator;

    protected override async Task OnInitializedAsync()
    {
        await base.CheckPermission(Permissions.Ticket.Edit);
        await LoadVenues();
        if (TicketId != 0)
        {
            await LoadTicket();
            await LoadVenueShowHalls(_ticket.VenueId);
            StateHasChanged();
        }

        _loaded = true;
        await base.OnInitializedAsync();
    }

    private async Task LoadTicket()
    {
        var response = await TicketManager.GetDetailAsync(new GetTicketByIdQuery()
        {
            FestivalId = SelectedFestivalId,
            Id = TicketId
        });
        if (response.Succeeded)
        {
            _ticket = Mapper.Map<AddEditTicketsCommand>(response.Data);
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await LoadVenues();
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadVenues()
    {
        await base.LoadSelectedFestivalId();
        var response = await VenueManager.GetAllVenue(new GetAllVenueQuery()
        {
            FestivalId = SelectedFestivalId,
            GetAllData = true
        });
        if (response.Succeeded)
        {
            _venues = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task ChangeVenueSelected(int venueId)
    {
        _ticket.VenueId = venueId;
        await LoadVenueShowHalls(venueId);
    }

    private async Task LoadVenueShowHalls(int venueId)
    {
        var response = await VenueManager.GetVenueDetail(new GetVenueByIdQuery()
        {
            Id = venueId,
            FestivalId = SelectedFestivalId
        });
        if (response.Succeeded)
        {
            _selectedVenue = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task SaveAsync()
    {
        _processing = true;
        var validated = _fluentValidationValidator.Validate(option => option.IncludeAllRuleSets());
        if (validated)
        {
            _ticket.ShowTimesId = _ticket.ShowTimesId.Where(p=>p!=0).ToList();
            await LoadSelectedFestivalId();
            _ticket.FestivalId=SelectedFestivalId;
            var response = await TicketManager.SaveTicketAsync(_ticket);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                await PendingApproval();
                GoToTicketsList();
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
    private async Task PendingApproval()
    {
        var options = new DialogOptions()
        {
            BackdropClick = true,
            CloseButton = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small
        };
        var d = await _dialogService.ShowAsync<ApprovedEmail>("Admin Approval", options);
        var res = await d.Result;
    }

    private void GoToTicketsList()
    {
        _navigationManager.NavigateTo(
            "/festival/tickets");
    }
}