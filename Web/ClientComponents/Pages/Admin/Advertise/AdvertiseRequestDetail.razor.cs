using System;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Advertises.Queries;
using HiSubmit.Client.Infrastructure.Managers.AdminAdvertise;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClientComponents.Pages.Admin.Advertise;

public partial class AdvertiseRequestDetail
{
    #region Parameters

    [Parameter] public int RequestId { get; set; }
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }

    #endregion

    #region Injection
  [Inject]
    private IAdminAdvertiseManager AdvertiseManager { get; set; }

    #endregion

    private bool _loaded;
    private GetDetailAdvertiseResponse _detail;

    protected override async Task OnInitializedAsync()
    {
        await LoadDetail();
        _loaded = true;
        await base.OnInitializedAsync();
    }

    private async Task LoadDetail()
    {
        var response = await AdvertiseManager.GetDetailAsync(new GetDetailAdvertiseRequest
        {
            Id = RequestId
        });
        if (response.Succeeded)
        {
            Console.WriteLine(response.Data.UserName);
            _detail = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
                MudDialog.Close();
            }
        }
    }

    private void Close()
    {
        MudDialog.Close();
    }
}