using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllOrginizer;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Web.Components.Pages.Public.Festivals.Components;

public partial class FestivalOrginizer
{
    [Parameter]
    public int FestivalId { get; set; }

    [Inject]
    public IPublicFestivalManager FestivalManager { get; set; } 
    
    
    private List<GetAllEventOrganizerResponse> Organizers { get; set; }

    private bool _loaded;

    private bool Loaded
    {
        get => _loaded;
        set
        {
            _loaded = value;
            Task.Run(async () =>
            {
                await CallJs();
            });
        }
    }

    private async Task CallJs()
    {
        // await _jsRuntime.InvokeVoidAsync("CreateImageSlider");
        // await _jsRuntime.InvokeVoidAsync("CreateOrganizerSlider");
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadOrganizer();
        await base.OnInitializedAsync();
        Loaded = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
          
        }
    }

    private async Task LoadOrganizer()
    {
        var response = await FestivalManager.GetAllOrganizerAsync(new GetAllOrganizerQuery()
        {
            FestivalId = FestivalId,
        });
        if (response.Succeeded)
        {
            Organizers = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message,MudBlazor.Severity.Error);
            }
        }
    }
}