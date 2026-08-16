using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetSubmitDetail;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubmit;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using Microsoft.AspNetCore.Components;

namespace ClientComponents.Shared.Components.Projects;

public partial class ProjectInformationCart
{
    private GetAllSubmitsResponse _submit;
    
    [Inject]
    private  IFestivalSubmitManager  SubmitManager { get; set; }

    [Parameter] public int SubmitId { get; set; }
    [Parameter] public int FestivalId { get; set; }

    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        await GetSubmitDetail();
        _loaded = true;
        await base.OnInitializedAsync();
    }

    private async Task GetSubmitDetail()
    {
        var response = await SubmitManager.GetSubmitDetailAsync(new GetSubmitDetailQuery
        {
            SubmitId = SubmitId,
            FestivalId = FestivalId
        });
        if (response.Succeeded)
            _submit = response.Data;
    }
}