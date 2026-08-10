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
        _festivalId =
            AuthenticationManager.GetMainFestivalId() ?? 0;


        if (await AuthenticationManager.GetSelectedFestivalId()!=null)
        {
            var selectedFestivalId = await AuthenticationManager.GetSelectedFestivalId();
            if (selectedFestivalId == null)
            {
                 SetFestivalIdToSelectedFestivalId();
            }
        }
        else
        {
             SetFestivalIdToSelectedFestivalId();
        }

        if (_festivalId == _selectedFestivalId && _festivalId != 0)
        {
            _canView = true;
        }

        var currentUser = await AuthenticationManager.CurrentUser();
        var festivalClaims = currentUser.Claims.FirstOrDefault(p => p.Type == ApplicationClaimTypes.FestivalPermission);
        if (festivalClaims != null)
        {
            var permissions = JsonSerializer.Deserialize<Dictionary<int, string[]>>(festivalClaims.Value);
            if (permissions != null && permissions.TryGetValue(_festivalId, out var permission))
            {
                if (permission.Any(p => p == Policy))
                    _canView=true;
            }
        }
    }

    private void SetFestivalIdToSelectedFestivalId()
    {
        _selectedFestivalId = AuthenticationManager.GetMainFestivalId() ?? 0;
    }
}