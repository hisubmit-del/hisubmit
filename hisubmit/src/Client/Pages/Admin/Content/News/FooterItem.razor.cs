using Hisubmit.Client.SharedModels.Features.FooterItems;
using Hisubmit.Client.SharedModels.Features.FooterItems.Commands;
using Hisubmit.Client.SharedModels.Features.FooterItems.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.Footer;
using HiSubmit.Client.Extensions;
using HiSubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace HiSubmit.Client.Pages.Admin.Content.News;
public partial class FooterItem
{
    [Inject] private IFooterManager FooterManager { get; set; }

    [CascadingParameter] private HubConnection HubConnection { get; set; }

    private List<FooterItemDto> _footerItems = new();
    private FooterItemDto _footerItem = new();
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
        await GetArtCategoryAsync();
        _loaded = true;
        HubConnection = HubConnection.TryInitialize(_navigationManager);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }

    private async Task GetArtCategoryAsync()
    {
        var response = await FooterManager.GetAllAsync(new GetAllFooterItemQuery());
        if (response.Succeeded)
        {
            _footerItems = response.Data.ToList();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task Delete(int id,string title)
    {
        string deleteContent = Localize["Delete Content"];
        var parameters = new DialogParameters
        {
            { nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, title) }
        };
        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(Localize["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await FooterManager.DeleteAsync(new DeleteFooterItemCommand(){Id = id});
            if (response.Succeeded)
            {
                await Reset();
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
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

    

    private async Task InvokeModal(FooterItemDto item)
    {
        var parameters = new DialogParameters();
        var model = new AddEditFooterItemCommand()
        {
            Id = item.Id,
            Link = item.Link,
            Title = item.Title,
            IsEnable = item.IsEnable
        };
        parameters.Add(nameof(AddEditFooterItem.AddEditFooterItemModel),model);
        
        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<AddEditFooterItem>(model.Id == 0 ? Localize["Create"] : Localize["Edit"],
            parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }

    private async Task Reset()
    {
        _footerItem = new FooterItemDto();
        await GetArtCategoryAsync();
        StateHasChanged();
    }

    private bool Search(FooterItemDto item)
    {
        if (string.IsNullOrWhiteSpace(_searchString)) return true;
        if (item.Title?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (item.Link?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }
}