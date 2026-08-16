using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllImages;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Pages.Festival.DashboardComponent;

public partial class FestivalImageSlider
{
    [Inject] private IFestivalManager FestivalManager { get; set; }

    [Parameter] public int FestivalId { get; set; }

    private List<GetAllFestivalImageResponse> _images = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadImages();
        await base.OnInitializedAsync();
    }

    private async Task LoadImages()
    {
        var response = await FestivalManager.GetAllImages(new GetAllFestivalImageQuery()
            { GetAllData = true, FestivalId = FestivalId });

        if (response.Succeeded)
        {
            _images = response.Data;
        }

        foreach (var message in response.Messages)
        {
            _snackBar.Add(message, Severity.Error);
        }
    }
}