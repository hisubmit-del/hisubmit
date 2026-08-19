using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Commands.AddProjectJudgingResult;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.CheckPermissionForJudging;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using HiSubmit.Client.Infrastructure.Managers.Referee;
using Web.Components.Pages.Festival.JudgingProjects;
using Web.Components.Pages.Festival.ProjectJudgings;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Pages.Public.Projects.ProjectDetails;

public partial class RateProject
{
    #region Injection

    [Inject] private IRefereeManager RefereeManager { get; set; }

    #endregion

    #region Parameter

    [Parameter] public GetProjectDetailResponse Project { get; set; }

    #endregion

    #region Private Filled

    private bool _loaded;
    private GetProjectJudgingDetailResponse _judgingDetail = new();
    private CheckPermissionResponse RefereePermission { get; set; } = new();
    private int _selectedJudgingId;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await CheckRefereePermission();
        await LoadReferee();
        _loaded = true;
        await base.OnInitializedAsync();
    }

    #endregion

    private async Task CheckRefereePermission()
    {
        var response = await RefereeManager.CheckPermission(Project.URL);
        if (response.Succeeded)
        {
            RefereePermission = response.Data;
            _selectedJudgingId = RefereePermission.Judgings.FirstOrDefault()?.Id ?? 0;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task ShowRefereeModal()
    {
        var selectedSubmit = RefereePermission.Judgings
            .FirstOrDefault(p => p.Id == _selectedJudgingId)
            ?? RefereePermission.Judgings.FirstOrDefault();
        if (selectedSubmit is null)
            return;
        var judgingResult = new AddEditProjectJudgingResultCommand
        {
            
            Id = selectedSubmit.Id,
            Comment = _judgingDetail.Comment,
            JudgingButtonId = _judgingDetail.JudgingButtonId,
            JudgingFiledAnswers = _judgingDetail.JudgingFiledAnswereds,
            SubmitAnswerQuestions = _judgingDetail.SubmitAnswerQuestions
        };
        var parameter = new DialogParameters
        {
            { nameof(ProjectJudgingForm.JudgingResult), judgingResult },
            { nameof(ProjectJudgingForm.ProjectType), Project.ProjectType },
            { nameof(ProjectJudgingForm.SubmitId), selectedSubmit.SubmitId },
            { nameof(ProjectJudgingForm.ProjectJudgingId), selectedSubmit.Id },
            { nameof(ProjectJudgingForm.FestivalId), selectedSubmit.FestivalId },
            { nameof(ProjectJudgingForm.FestivalName), selectedSubmit.FestivalName },
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            
        };
      var dialog=  _dialogService.Show<ProjectJudgingForm>(Localize["Submit Judge"], parameter, options);
      var res =await dialog.Result;
      if (!res.Canceled && res.Data is true)
      {
          _snackBar.Add(Localize["Judgment submitted"], Severity.Success);
      }
      await LoadReferee();
      StateHasChanged();
    }

    private async Task LoadReferee()
    {
        var selectedSubmit = RefereePermission.Judgings
            .FirstOrDefault(p => p.Id == _selectedJudgingId)
            ?? RefereePermission.Judgings.FirstOrDefault();
        if (selectedSubmit != null)
        {
            var user = await AuthenticationManager.CurrentUser();
            if (user.Identity is { IsAuthenticated: true })
            {
                var response = await RefereeManager.GetProjectJudgingDetail(new GetProjectJudgingDetailQuery()
                {
                    Id = selectedSubmit.Id
                });
                if (response.Succeeded)
                    _judgingDetail = response.Data;
            }   
        }
    }

    private async Task SelectJudging(int judgingId)
    {
        _selectedJudgingId = judgingId;
        await LoadReferee();
    }
}
