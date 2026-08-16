using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Services;
using Microsoft.AspNetCore.Components;

namespace HiSubmit.Web.Components.Pages.Admin;

public class BaseAdminPages:ComponentBase
{
    [Inject]
    private  MainLayoutService MainLayoutService { get; set; }

    protected override Task OnInitializedAsync()
    {
        MainLayoutService.ChangeDrawerStatus(true);
        return base.OnInitializedAsync();
    }
}