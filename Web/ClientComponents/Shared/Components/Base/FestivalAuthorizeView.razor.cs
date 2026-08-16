using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;

namespace ClientComponents.Shared.Components.Base;

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
            await _localStorage.GetItemAsync<int>(StorageConstants.Local.FestivalId);

        if (await _localStorage.ContainKeyAsync(StorageConstants.Local.SelectedFestivalId))
        {
            var selectedFestivalId = await _localStorage.GetItemAsync<int?>
                (StorageConstants.Local.SelectedFestivalId);
            if (selectedFestivalId != null)
            {
                _selectedFestivalId = 
                    await _localStorage.GetItemAsync<int>(StorageConstants.Local.SelectedFestivalId);
            }
            else
            {
                await SetFestivalIdToSelectedFestivalId();
            }
        }
        else
        {
            await SetFestivalIdToSelectedFestivalId();
        }
        if (_festivalId == _selectedFestivalId && _festivalId != 0)
        {
            _canView = true;
        }
        
        var currentUser = await AuthenticationManager.CurrentUser();
        
        var festivalPermissions =
            currentUser.Claims.Where(p => p.Type == ApplicationClaimTypes.FestivalPermission).ToList();

        var neededPolicy = $"{_selectedFestivalId}-{Policy}";

        if (festivalPermissions.Any(p => p.Value == neededPolicy))
        {
            _canView = true;
        }
    }

    private async Task SetFestivalIdToSelectedFestivalId()
    {
        _selectedFestivalId=  
            await _localStorage.GetItemAsync<int>(StorageConstants.Local.FestivalId);
    }
}