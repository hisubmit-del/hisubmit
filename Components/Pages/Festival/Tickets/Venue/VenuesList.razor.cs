using System;
using MudBlazor;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Authorization;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using HiSubmit.Client.Infrastructure.Managers.Venues;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetVenueById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllShowHall;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditShowHall;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalVenue;
using Web.Components.Pages.Festival.FestivalEditComponent;

namespace Web.Components.Pages.Festival.Tickets.Venue;

//[Authorize(Policy = $"{Permissions.Venue.View}")]
public partial class VenuesList
{
    #region Inject

    [Inject] private IMapper Mapper { get; set; }
    [Inject] public IVenueManager VenueManager { get; set; }
    [Inject] public IFestivalManager FestivalManager { get; set; }

    #endregion
    

    private List<GetAllVenueResponse> _venues;
    

    private List<GetAllVenueResponse> _pagedDate;
    public GetAllVenueQuery Query { get; set; }


    private MudTable<GetAllVenueResponse> _table;

    private List<GetAllShowHallResponse> _showHalls = new();
    private GetAllVenueQuery _advancedSearch = new GetAllVenueQuery();
    private int _totalItems;
    private int _currentPage;
    private string _searchString = "";
    private bool _openSearchForm = false;
    private bool _isAdvancedSearch = false;
    private bool _loaded;


    private bool _showHallProcessing;
    private Dictionary<int, bool> ShowDetails { get; set; } = new();

    private async Task LoadVenueDetail(int venueId)
    {
        if (!ShowDetails[venueId])
        {
            HideAllDetail();
            _showHallProcessing = true;
            var response = await VenueManager.GetVenueDetail(new GetVenueByIdQuery()
            {
                Id = venueId,
                FestivalId = SelectedFestivalId
            });
            if (response.Succeeded)
            {
                _showHalls = response.Data.ShowHalls;

                _showHallProcessing = false;
                ShowDetails[venueId] = true;
                StateHasChanged();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
        else
        {
            HideAllDetail();
        }
    }

    private void HideAllDetail()
    {
        foreach (var venue in _pagedDate)
        {
            ShowDetails[venue.Id] = false;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await CheckPermission(Permissions.Venue.View);
        await base.OnInitializedAsync();
        _loaded = true;
    }

    private void GenerateShowDetailDictionary()
    {
        foreach (var data in _pagedDate)
        {
            ShowDetails.Add(data.Id, false);
        }
    }

    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
    }

    private async Task<TableData<GetAllVenueResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllVenueQuery();
        if (_isAdvancedSearch)
        {
            query = _advancedSearch;
        }
        else
        {
            query.SearchString = _searchString;
        }

        await LoadData(state.Page, state.PageSize, state, query);
        ShowDetails.Clear();
        GenerateShowDetailDictionary();
        return new TableData<GetAllVenueResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        _isAdvancedSearch = true;
        await _table.ReloadServerData();
        _isAdvancedSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllVenueQuery query)
    {
        await base.LoadSelectedFestivalId();
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        query.FestivalId = SelectedFestivalId;
        var response = await VenueManager.GetAllVenue(query);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(query.SearchString))
                    return true;
                if (element.Name != null &&
                    element.Name.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                // if (element.Address != null && element.Name.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                //     return true;
                return false;
            });
            switch (state.SortLabel)
            {
                case "Name":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Name);
                    break;
                // case "SubmitDateFrom":
                //     loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.SubmitDateFrom);
                //     break;
            }

            data = loadedData.ToList();
            _pagedDate = data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private async Task AddShowHall(int venueId)
    {
        var parameters = new DialogParameters();
        var showHall = new AddEditShowHallCommand() { VenueId = venueId };
        parameters.Add(nameof(AddEditShowHall.ShowHall), showHall);
        var options = new DialogOptions
            {
                CloseButton = true, 
                MaxWidth = MaxWidth.Medium,
                FullWidth = true, 
                // 
            };
        var dialog = _dialogService.Show<AddEditShowHall>(Localize["Add Show Hall"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task AddVenue()
    {
        var parameters = new DialogParameters();
        var venue = new AddEditFestivalVenueCommand() { FestivalId = SelectedFestivalId };

        parameters.Add(nameof(AddEditEventVenueModal.Venue), venue);
        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, 
             //    
            };
        var dialog = _dialogService.Show<AddEditEventVenueModal>(Localize["Add Show Hall"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }


    private async Task EditVenue(int id)
    {
        var response = await VenueManager.GetVenueDetail(new GetVenueByIdQuery() { Id = id, FestivalId = SelectedFestivalId });
        if (response.Succeeded)
        {
            var editHall = Mapper.Map<AddEditFestivalVenueCommand>(response.Data);
            var parameters = new DialogParameters
                { { nameof(AddEditEventVenueModal.Venue), editHall } };

            var options = new DialogOptions
                { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
            var dialog = _dialogService.Show<AddEditEventVenueModal>(Localize["Edit Venue"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await _table.ReloadServerData();
            }
        }
    }

    private async Task EditShowHall(int showHallId)
    {
        var showHall = _showHalls.FirstOrDefault(p => p.Id == showHallId);
        var showHallCommand = Mapper.Map<AddEditShowHallCommand>(showHall);
        var parameters = new DialogParameters
            { { nameof(AddEditShowHall.ShowHall), showHallCommand } };

        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true,  };
        var dialog = _dialogService.Show<AddEditShowHall>(Localize["Edit Show Hall"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }
}