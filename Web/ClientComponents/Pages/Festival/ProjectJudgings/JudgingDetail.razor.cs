using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Judgings.Queries.Detail;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetDetail;
using HiSubmit.Client.Infrastructure.Managers.Judgings;
using HiSubmit.Client.Infrastructure.Managers.Referee;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClientComponents.Pages.Festival.ProjectJudgings;

public partial class JudgingDetail
{
    #region Injects

    [Inject] private IRefereeManager RefereeManager { get; set; }
    [Inject] private IJudgingManager JudgingManager { get; set; }

    #endregion
    
    

    [Parameter] public int Id { get; set; }

    private GetProjectJudgingDetailResponse _projectJudging;
    private GetJudgingDetailResponse _judgingForm;
    private bool _loaded = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadJudgingDetail();
        await LoadJudgingQuestion();
        await base.OnInitializedAsync();
        _loaded = true;
    }

    private async Task LoadJudgingDetail()
    {
        var response = await RefereeManager.GetProjectJudgingDetail(new GetProjectJudgingDetailQuery
        {
            Id = Id
        });
        if (response.Succeeded)
        {
            _projectJudging = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task LoadJudgingQuestion()
    {
        var response = await JudgingManager.GetDetail(new GetJudgingDetailQuery
        {
            ProjectType = _projectJudging.Submit.ProjectProjectType,
            FestivalId = _projectJudging.Submit.FestivalId
        });
        if (response.Succeeded)
        {
            _judgingForm = response.Data;
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