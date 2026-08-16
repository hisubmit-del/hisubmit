using AutoMapper;
using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetVenueById;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Hisubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiSubmit.Web.Components.Pages.Festival.FestivalEditComponent;

public partial class FestivalVenue
{
    [Inject]
    private IFestivalManager FestivalManager { get; set; }
    [Inject]
    private ILocalStorageService LocalStorageService { get; set; }
    [Inject]
    private IMapper Mapper { get; set; }
    [Parameter]
    public int FestivalId { get; set; }

    [Parameter]
    public  bool ReadOnlyMood { get; set; }
    [Parameter]
    public  bool IsAdmin { get; set; }
    private List<GetAllVenueResponse> _venues { get; set; }

    private bool _loaded = true;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        // await GetFestivalId();
        await GetVenues();
    }

    private async Task GetVenues()
    {
        var result = await FestivalManager.GetAllVenueAsync(new GetAllVenueQuery
        {
            FestivalId = FestivalId,
            GetAllData = true
        });
        if (result.Succeeded)
        {
            _venues = result.Data;
        }
        else
        {
            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
    //private async Task GetFestivalId()
    //{
    //    FestivalId = await LocalStorageService.GetItemAsync<int>(StorageConstants.Local.FestivalId);
    //}

    public async Task InvokeModal(int id=0)
    {
        var title = "";
        AddEditFestivalVenueCommand venue = null;

        if (id == 0)
        {
            title = Localize["Add Venue"];
            venue = new AddEditFestivalVenueCommand()
            {
                FestivalId = FestivalId,
                Address = new AddEditAddressCommand()
            };
        }
        else
        {
            title = Localize["Updated Venue"];
            var venueDetail =await FestivalManager.GetVenueById(new GetVenueByIdQuery
            {
                Id = id,
                FestivalId=FestivalId
            });
            venue = Mapper.Map<AddEditFestivalVenueCommand>(venueDetail.Data);
        }

        var parameter = new DialogParameters();
        parameter.Add(nameof(AddEditEventVenueModal.Venue), venue);
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            
        };
        var dialog = _dialogService.Show<AddEditEventVenueModal>(title, parameter, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }
    private async Task DeleteAsync(int id, string name)
    {
        string deleteContent = name;
        var parameters = new DialogParameters
        {
            {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(Localize["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await FestivalManager.DeleteVenueAsync(new DeleteVenueCommand { Id = id ,FestivalId=FestivalId});
            if (response.Succeeded)
            {
                await Reset();
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                await Reset();
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
    private async Task Reset()
    {
        _venues = new List<GetAllVenueResponse>();
        await GetVenues();
        StateHasChanged();
    }


}