using Hisubmit.Client.SharedModels.Features.Brands.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.GetAll;
using HiSubmit.Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Catalog.Brand;

using Hisubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HiSubmit.Web.Components.Pages.Admin.Catalog
{
    public partial class ArtCategories
    {
        [Inject] private IArtCategoryManager ArtCategoryManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private List<GetAllArtCategoryResponse> _categoryList = new();
        private GetAllArtCategoryResponse _category = new();
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
            var response = await ArtCategoryManager.GetAllAsync();
            if (response.Succeeded)
            {
                _categoryList = response.Data.ToList();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task Delete(int id)
        {
            string deleteContent = Localize["Delete Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(Localize["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var response = await ArtCategoryManager.DeleteAsync(id);
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

        private async Task ExportToExcel()
        {
            var response = await ArtCategoryManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"ArtCategory_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? Localize["ArtCategory exported"]
                    : Localize["Filtered ArtCategory exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                _category = _categoryList.FirstOrDefault(c => c.Id == id);
                if (_category != null)
                {
                    parameters.Add(nameof(AddEditArtCategoryModal.ArtCategoryModel), new AddEditArtCatgoryRequest
                    {
                        Id = _category.Id,
                        Name = _category.Name,
                        Description = _category.Description,
                        //Tax = _focus.Tax
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
            var dialog = _dialogService.Show<AddEditArtCategoryModal>(id == 0 ? Localize["Create"] : Localize["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _category = new GetAllArtCategoryResponse();
            await GetArtCategoryAsync();
        }

        private bool Search(GetAllArtCategoryResponse brand)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (brand.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (brand.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return false;
        }
    }
}