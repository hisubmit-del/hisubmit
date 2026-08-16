using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using Microsoft.AspNetCore.Components;

namespace HiSubmit.Web.Components.Pages.Festival.Submits;

public partial class SubmitFilterForm
{
    [Parameter]
    public GetAllSubmitsRequest Model { get; set; }
    
    [Parameter]
    public EventCallback<GetAllSubmitsRequest> ModelChanged { get; set; }

    [Parameter]
    public EventCallback OnSearchClicked { get; set; }

    private async Task Serach()
    {
        await OnSearchClicked.InvokeAsync();
    }

    private async Task CancelSearch()
    {
        await OnSearchClicked.InvokeAsync();
    }
}