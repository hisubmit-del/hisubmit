using FluentValidation;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Commands.AddEditFestivalFocus;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Commands.DeleteFestivalFocus;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Queries.GetAllFestivalFocus;
using HiSubmit.Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Catalog.FestivalFocus;
using Hisubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HiSubmit.Web.Components.Pages.Admin.FestivalFocuses;

[Authorize(Policy = Permissions.FocusCategory.View)]
public partial class FestivalFocus
{

    [Inject] private IFestivalFocusManager FestiivalFocusManager { get; set; }

    [CascadingParameter] private HubConnection HubConnection { get; set; }

    private List<GetAllFestivalFocusResponse> _festivalList = new();
    private GetAllFestivalFocusResponse _focus = new();
    private string _searchString = "";
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;

    private ClaimsPrincipal _currentUser;
    private bool _canCreateArtCategory;
    private bool _canEditArtCategory;
    private bool _canDeleteArtCategory;
    private bool _canExportArtCategory;
    private bool _canSearchArtCategory;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _currentUser = await AuthenticationManager.CurrentUser();
        _canCreateArtCategory = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ArtCategory.Create)).Succeeded;
        _canEditArtCategory = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ArtCategory.Edit)).Succeeded;
        _canDeleteArtCategory = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ArtCategory.Delete)).Succeeded;
        _canExportArtCategory = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ArtCategory.Export)).Succeeded;
        _canSearchArtCategory = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ArtCategory.Search)).Succeeded;

        await GetFestivalFocusAsync();
        _loaded = true;
        HubConnection = HubConnection.TryInitialize(_navigationManager);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }

    private async Task GetFestivalFocusAsync()
    {
        var response = await FestiivalFocusManager.GetAllAsync(new GetAllFestivalFocusQuery());
        if (response.Succeeded)
        {
            _festivalList = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }

    private async Task Delete(int id,string name)
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
            var response = await FestiivalFocusManager.DeleteAsync(new DeleteFestivalFocusCommand { Id=id});
            if (response.Succeeded)
            {
                await Reset();
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                _snackBar.Add(response.Messages[0], MudBlazor.Severity.Success);
            }
            else
            {
                await Reset();
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, MudBlazor. Severity.Error);
                }
            }
        }
    }

    //private async Task ExportToExcel()
    //{
    //    var response = await FestiivalFocusManager.ExportToExcelAsync(_searchString);
    //    if (response.Succeeded)
    //    {
    //        await _jsRuntime.InvokeVoidAsync("Download", new
    //        {
    //            ByteArray = response.Data,
    //            FileName = $"{nameof(ArtCategory).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
    //            MimeType = ApplicationConstants.MimeTypes.OpenXml
    //        });
    //        _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
    //            ? _localizer["ArtCategory exported"]
    //            : _localizer["Filtered ArtCategory exported"], Severity.Success);
    //    }
    //    else
    //    {
    //        foreach (var message in response.Messages)
    //        {
    //            _snackBar.Add(message, Severity.Error);
    //        }
    //    }
    //}

    private async Task InvokeModal(int id = 0)
    {
        var parameters = new DialogParameters();
        if (id != 0)
        {
            _focus = _festivalList.FirstOrDefault(c => c.Id == id);
            if (_focus != null)
            {
                parameters.Add(nameof(AddEditFestivalFocusModal.FestivalFocus), new AddEditFestivalFocusCommand
                {
                    Id = _focus.Id,
                    Name = _focus.Name,
                    Description = _focus.Description,
                    //Tax = _focus.Tax
                });
            }
        }
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<AddEditFestivalFocusModal>(id == 0 ? Localize["Create"] : Localize["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }

    private async Task Reset()
    {
        _focus = new GetAllFestivalFocusResponse();
        await GetFestivalFocusAsync();
    }

    //private bool Search(GetAllArtCategoryyResponse brand)
    //{
    //    if (string.IsNullOrWhiteSpace(_searchString)) return true;
    //    if (brand.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
    //    {
    //        return true;
    //    }
    //    if (brand.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
    //    {
    //        return true;
    //    }
    //    return false;
    //}
}