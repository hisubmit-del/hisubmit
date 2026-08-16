using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAll;
using Microsoft.AspNetCore.Components;

namespace ClientComponents.Pages.Project;

public partial class ProjectFilterForm
{
    [Parameter]
    public GetAllProjectRequest Model { get; set; }
    [Parameter]
    public EventCallback<GetAllProjectRequest> ModelChanged { get; set; }
    [Parameter]
    public EventCallback OnSubmitClicked { get; set; }

    private async Task Serach()
    {
        await OnSubmitClicked.InvokeAsync();
    }
}