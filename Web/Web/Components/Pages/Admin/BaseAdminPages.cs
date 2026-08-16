using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace Web.Components.Pages.Admin;

public class BaseAdminPages : ComponentBase
{
    [Inject] private MainLayoutService MainLayoutService { get; set; }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        MainLayoutService.ChangeDrawerStatus(true);
        await base.OnInitializedAsync();
    }

}
