using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace Web.Components.Shared.Components.Base;

public partial class FestivalAuthorizeView
{
    #region Parameters

    [Parameter] public string Policy { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }

    #endregion

    private bool _canView = false;

    private int _festivalId;
    private int _selectedFestivalId;

    protected override async Task OnInitializedAsync()
    {
        _festivalId = AuthenticationManager.GetMainFestivalId() ?? 0;
        _selectedFestivalId =
            await AuthenticationManager.GetAdminLoginToFestivalId() ??
            await AuthenticationManager.GetSelectedFestivalId() ??
            _festivalId;

        var currentUser = await AuthenticationManager.CurrentUser();
        if (currentUser.IsInRole(HiSubmit.Client.SharedModels.Constants.Role.RoleConstants.AdministratorRole))
        {
            _canView = true;
        }
        else
        {
            foreach (var festivalClaims in currentUser.Claims
                         .Where(p => p.Type == ApplicationClaimTypes.FestivalPermission))
            {
                try
                {
                    var permissions = JsonSerializer.Deserialize<Dictionary<int, string[]>>(festivalClaims.Value);
                    if (permissions != null &&
                        permissions.TryGetValue(_selectedFestivalId, out var permission) &&
                        permission?.Any(p => string.Equals(p, Policy, StringComparison.OrdinalIgnoreCase)) == true)
                    {
                        _canView = true;
                        break;
                    }
                }
                catch (JsonException)
                {
                    // Invalid claims are treated as no access.
                }
            }
        }
    }
}
