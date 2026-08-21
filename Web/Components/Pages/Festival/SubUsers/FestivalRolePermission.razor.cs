using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Hisubmit.Client.SharedModels.Requests.Identity;
using Hisubmit.Client.SharedModels.Responses.Identity;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubUsers;
using HiSubmit.Client.Infrastructure.Managers.Identity.Roles;
using HiSubmit.Client.Infrastructure.Mappings;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.CustomeAttribute;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging.Abstractions;
using MudBlazor;

namespace Web.Components.Pages.Festival.SubUsers
{
    public partial class FestivalRolePermission
    {
        [Inject] private IRoleManager RoleManager { get; set; }

        [Inject] private IFestivalSubUserManager FestivalSubUserManager { get; set; }

        private PermissionType PermissionType { get; set; } = PermissionType.Admin;
        [Parameter] public string Id { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public string Description { get; set; }

        private PermissionResponse _model;
        private Dictionary<string, List<RoleClaimResponse>> GroupedRoleClaims { get; } = new();
        private IMapper _mapper;
        private RoleClaimResponse _roleClaims = new();
        private RoleClaimResponse _selectedItem = new();
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canEditRolePermissions;
        private bool _canSearchRolePermissions;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await AuthenticationManager.CurrentUser();
            _canEditRolePermissions =
                (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.Edit)).Succeeded
                || (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.SubUserRole.Edit))
                .Succeeded;


            _canSearchRolePermissions =
                (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.Search)).Succeeded
                || (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.SubUserRole.Edit))
                .Succeeded;
            ;

            PermissionType = (await _authorizationService
                .AuthorizeAsync(_currentUser, Permissions.RoleClaims.View)).Succeeded
                ? PermissionType.Admin
                : PermissionType.Festival;


            await GetRolePermissionsAsync();
            _loaded = true;
         //   HubConnection = HubConnection.TryInitialize(_navigationManager);
            // if (HubConnection.State == HubConnectionState.Disconnected)
            // {
            //     await HubConnection.StartAsync();
            // }
        }

        private async Task GetRolePermissionsAsync()
        {
            _mapper = new MapperConfiguration(c => { c.AddProfile<RoleProfile>(); },
                NullLoggerFactory.Instance).CreateMapper();
            var roleId = Id;
            IResult<PermissionResponse> result;

            result = await FestivalSubUserManager.GetAllPermission(roleId);


            if (result.Succeeded)
            {
                _model = result.Data;
                GroupedRoleClaims.Add(Localize["All Permissions"], _model.RoleClaims);
                foreach (var claim in _model.RoleClaims)
                {
                    if (GroupedRoleClaims.ContainsKey(claim.Group))
                    {
                        GroupedRoleClaims[claim.Group].Add(claim);
                    }
                    else
                    {
                        GroupedRoleClaims.Add(claim.Group, new List<RoleClaimResponse> {claim});
                    }
                }

                if (_model != null)
                {
                    Description = string.Format(Localize["Manage {0} 's Permissions"],
                        _model.RoleName);
                }
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }

                _navigationManager.NavigateTo("/identity/roles");
            }
        }

        private async Task SaveAsync()
        {
            var request = _mapper.Map<PermissionResponse, PermissionRequest>(_model);
            var result = await FestivalSubUserManager.UpdatePermissionsAsync(request);
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
             
                _navigationManager.NavigateTo("/festival/subUser/roles");
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }


        private bool Search(RoleClaimResponse roleClaims)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (roleClaims.Value?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (roleClaims.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return false;
        }

        private Color GetGroupBadgeColor(int selected, int all)
        {
            if (selected == 0)
                return Color.Error;

            if (selected == all)
                return Color.Success;

            return Color.Info;
        }
    }
}
