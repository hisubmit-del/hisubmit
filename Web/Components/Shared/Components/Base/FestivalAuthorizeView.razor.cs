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

        if (_selectedFestivalId != 0 && _festivalId == _selectedFestivalId)
        {
            _canView = true;
        }

        var currentUser = await AuthenticationManager.CurrentUser();
        var festivalClaims = currentUser.Claims.FirstOrDefault(p => p.Type == ApplicationClaimTypes.FestivalPermission);
        if (festivalClaims != null)
        {
            var permissions = JsonSerializer.Deserialize<Dictionary<int, string[]>>(festivalClaims.Value);
            if (permissions != null && permissions.TryGetValue(_selectedFestivalId, out var permission))
            {
                if (permission.Any(p => p == Policy))
                    _canView=true;
            }
        }
    }
}
