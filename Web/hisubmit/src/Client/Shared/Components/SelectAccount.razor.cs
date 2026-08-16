using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalNames;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using HiSubmit.Client.SharedModels.Constants.Role;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Shared.Components;

public partial class SelectAccount
{
    [Inject] private IFestivalManager FestivalManager { get; set; }

    private bool _isFestivalUser;
    private  int? SelectedFestivalId { get; set; }
    private List<GetFestivalNamesResponse> FestivalNames { get; set; }
    private GetFestivalDetailResponse Festival;
    private  int FestivalId { get; set; }
    protected override async Task OnParametersSetAsync()
    {
        FestivalId=await _localStorage.GetItemAsync<int>(StorageConstants.Local.FestivalId);
        if (FestivalId != 0)
        {
            await LoadFestivalData();
        }

        await LoadOtherFestival();
    }

    private async Task LoadFestivalData()
    {
        var response = await FestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
            Festival = response.Data;
    }

    private async Task LoadOtherFestival()
    {
        var currentUser = await AuthenticationManager.CurrentUser();
        var festivalRoles = currentUser.Claims.Where(p => p.Type == ApplicationClaimTypes.FestivalRole).ToList();

        if (!festivalRoles.Any())
            return;
        
        var festivalIds = festivalRoles.Select(p => int.Parse(p.Value.Split('-').First())).ToList();
        await GetFestivalNames(festivalIds);
    }
    private async Task GetFestivalNames(List<int> festivalIds)
    {
        var response = await FestivalManager.GetFestivalNames(new GetFestivalNamesQuery()
        {
            FestivalIdString =string.Join(',',festivalIds)
        });

        if (response.Succeeded)
        {
            FestivalNames = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
}