using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditShowHall;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllShowHall;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Commands;
using HiSubmit.Client.Infrastructure.Managers.SoldTickets;
using HiSubmit.Client.Infrastructure.Managers.Venues;
using HiSubmit.Client.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Shared.Components;

public partial class AddTicketToCartModal
{
    [Inject] private ISoldTicketManager SoldTicketManager { get; set; }
    [Inject] private IVenueManager VenueManager { get; set; }
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public AddSoldTicketCommand SoldTicket { get; set; }
    [Parameter]
    public int MaxCount { get; set; }

    [Inject]
    public UserCartService UserCartService { get; set; }

    private List<GetAllShowHallResponse> _showHalls { get; set; } = new();
    private GetAllShowHallResponse _selectedShowHall = new();

    private int? _selectedShowHallId;
    private int? SelectedShowHallId
    {
        get => _selectedShowHallId; set
        {
            _selectedShowHallId=value;
            if (_selectedShowHallId !=null)
                _selectedShowHall=_showHalls.FirstOrDefault(p => p.Id==_selectedShowHallId);
            StateHasChanged();
        }
    }


    private FluentValidationValidator _fluentValidationValidator;
    private bool _processing;

    private HashSet<ShowTimeDto> _selectedItems;
    private HashSet<ShowTimeDto> SelectedItems { get; set; } = new();
    protected override async Task OnInitializedAsync()
    {
        await LoadShowHalls();
        //  _selectedShowHall = _showHalls.First(); 
        await base.OnInitializedAsync();
    }


    private async Task SaveAsync()
    {
        if (!SelectedItems.Any())
        {
            return;
        }

        SoldTicket.ShowTimeId = SelectedItems.First().Id;
        var validated = _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (validated)
        {
            _processing = true;
            var response = await SoldTicketManager.AddTicketToCart(SoldTicket);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                UserCartService.ChangeUserCart();

                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            _processing = false;
        }
    }

    private async Task LoadShowHalls()
    {
        var response = await VenueManager.GetAllShowHalls(new GetAllShowHallQuery()
        {
            VenueId = SoldTicket.VenueId
        });
        if (response.Succeeded)
        {
            _showHalls = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task DeleteBeforeItem(HashSet<ShowTimeDto> showTimes)
    {
        Console.WriteLine(showTimes.Count);
        Console.WriteLine(SelectedItems.Count);
        var s = showTimes.Where(p => SelectedItems.Any(k => k == p));
        Console.WriteLine("s:{0}", s.Count());
        if (showTimes.Count > 1)
        {

            SelectedItems.Remove(s.First());
        }
    }

    private void Cancel()
    {
        MudDialog.Close();
    }
}