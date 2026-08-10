using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace Web.Components.Pages.User;

public class BaseUserPages:ComponentBase
{
    [Inject]
    private  MainLayoutService MainLayoutService { get; set; }
    protected override  Task OnInitializedAsync()
    {
         MainLayoutService.ChangeDrawerStatus(true);
        return base.OnInitializedAsync();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        MainLayoutService.ChangeDrawerStatus(true);
        return base.OnAfterRenderAsync(firstRender);
    }
}
