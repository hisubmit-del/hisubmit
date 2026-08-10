using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Advertises.Queries;
using HiSubmit.Client.Infrastructure.Managers.AdminAdvertise;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Pages.Admin.Advertise;

[Authorize(Policy=Permissions.Advertise.RequestView)]
public partial class AdvertiseDetail
{
    #region Inject

    [Inject]
    private IAdminAdvertiseManager AdminAdvertiseManager { get; set; }
    
    #endregion

    #region  Parameter

    [Parameter]
    public  int AdvertiseId { get; set; }

    #endregion

    #region Private Feild

    private GetDetailAdvertiseResponse _advertise=new();

    #endregion

    #region  Override

    protected  override  async Task OnInitializedAsync()
    {
        await LoadAdvertise();
        await  base.OnInitializedAsync();
    }

    #endregion
    private  async  Task LoadAdvertise()
    {
        var response = await AdminAdvertiseManager.GetDetailAsync(new GetDetailAdvertiseRequest
        {
            Id = AdvertiseId
        });
        if (response.Succeeded)
            _advertise = response.Data;
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
    }
}