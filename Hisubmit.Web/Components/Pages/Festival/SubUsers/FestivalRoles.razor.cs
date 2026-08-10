using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Requests.Identity;
using Hisubmit.Client.SharedModels.Responses.Identity;
using HiSubmit.Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubUsers;
using HiSubmit.Client.Infrastructure.Managers.Identity.Roles;
using HiSubmit.Web.Components.Pages.Identity;
using Hisubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace HiSubmit.Web.Components.Pages.Festival.SubUsers
{
    public partial class FestivalRoles
    {
        #region Inject

        [Inject] private IRoleManager RoleManager { get; set; }

        [Inject] public IFestivalSubUserManager FestivalSubUserManager { get; set; }

        #endregion

        #region Parameters

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        #endregion


        private bool _loaded;
        private bool _striped = true;
        private string _searchString = "";
        private RoleResponse _role = new();
        private List<RoleResponse> _roleList = new();

        protected override async Task OnInitializedAsync()
        {
            await CheckPermission(Permissions.SubUserRole.View);
            await GetRolesAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task GetRolesAsync()
        {
            var response = await FestivalSubUserManager.GetFestivalRoleAsync(SelectedFestivalId);

            if (response.Succeeded)
            {
                _roleList = response.Data.ToList();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task Delete(string id)
        {
            string deleteContent = Localize["Delete Content"];
            var parameters = new DialogParameters
            {
                { nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id) }
            };
            var options = new DialogOptions
                { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
            var dialog =
                _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(Localize["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var response = await RoleManager.DeleteAsync(id);
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


        private async Task InvokeModal(string id = null)
        {
            var parameters = new DialogParameters {
                { nameof(RoleModal.IsFestivalRole), true } 
            };

            if (id != null)
            {
                _role = _roleList.FirstOrDefault(c => c.Id == id);
                if (_role != null)
                {
                    parameters.Add(nameof(RoleModal.RoleModel), new RoleRequest
                    {
                        Id = _role.Id,
                        Name = _role.Name,
                        Description = _role.Description
                    });
                }
            }

            var options = new DialogOptions
                { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
            var dialog = _dialogService.Show<RoleModal>(id == null ? Localize["Create"] : Localize["Edit"], parameters,
                options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await Reset();
            }
        }

        private async Task Reset()
        {
            _role = new RoleResponse();
            await GetRolesAsync();
        }

        private bool Search(RoleResponse role)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (role.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (role.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return false;
        }

        private void ManagePermissions(string roleId)
        {
            _navigationManager.NavigateTo($"/festival/subUser/role-permissions/{roleId}");
        }
    }
}