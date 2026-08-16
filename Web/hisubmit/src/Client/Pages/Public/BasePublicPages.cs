using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HiSubmit.Client.Pages.Public;

[AllowAnonymous]
public class BasePublicPages:ComponentBase
{
    [Inject]
    private  MainLayoutService MainLayoutService { get; set; }
    protected override Task OnInitializedAsync()
    {
        MainLayoutService.ChangeDrawerStatus(false);
        return base.OnInitializedAsync();
    }
}

