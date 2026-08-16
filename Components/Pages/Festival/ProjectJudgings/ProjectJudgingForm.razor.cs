using Hisubmit.Client.SharedModels.Features.Judgings.Queries.Detail;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Commands.AddProjectJudgingResult;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;
using Hisubmit.Client.SharedModels.Requests.AnswerQuestions;
using HiSubmit.Client.Infrastructure.Managers.Judgings;
using HiSubmit.Client.Infrastructure.Managers.Referee;
using HiSubmit.Client.Infrastructure.Managers.Submissiions;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Components.Pages.Festival.ProjectJudgings;

public partial class ProjectJudgingForm
{
    #region Inject

    [Inject] public ISubmitManager SubmitManager { get; set; }
    [Inject] public IJudgingManager JudgingManager { get; set; }
    [Inject] public IRefereeManager RefereeManager { get; set; }
    [Inject] public ISubmissionQuestionManager SubmissionQuestionManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int SubmitId { get; set; }
    [Parameter] public int FestivalId { get; set; }
    [Parameter] public int ProjectJudgingId { get; set; }
    [Parameter] public ProjectType ProjectType { get; set; }
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }
    [Parameter] public AddEditProjectJudgingResultCommand JudgingResult { get; set; }

    #endregion

    #region private Filed

    private bool _loaded;
    private string _buttonText;
    private Dictionary<int, int> _filedRates ;
    
    private GetJudgingDetailResponse _judgingForm;
    private List<GetAllSubmissionQuestionResponse> _questions;

    #endregion

    protected override async Task OnInitializedAsync()
    {
        await LoadJudgingForm();
        if (JudgingResult.JudgingButtonId != null)
        {
            var button = _judgingForm.JudgingButtons
                .FirstOrDefault(p => p.Id == JudgingResult.JudgingButtonId);
            if (button != null)
            {
                SetButtonId(button.Id, button.Name);
            }
        }
        await base.OnInitializedAsync();
        await LoadQuestions();

        GenerateRatingDictionary();

        _loaded = true;
    }

    private async Task LoadJudgingForm()
    {
        var response = await JudgingManager.GetDetail(new GetJudgingDetailQuery()
        {
            FestivalId = FestivalId,
            ProjectType = ProjectType
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


    private void GenerateRatingDictionary()
    {
        _filedRates = new Dictionary<int, int>();
        foreach (var filed in _judgingForm.JudgingFileds)
        {
            var judgingField = JudgingResult.JudgingFiledAnswers
                .FirstOrDefault(p => p.JudgingFiledId == filed.Id);

            _filedRates.Add(filed.Id, judgingField?.Rate ?? 0);
        }
    }

    private void SetButtonId(int selectedButtonId, string selectedButtonText)
    {
        _buttonText = selectedButtonText;
        JudgingResult.JudgingButtonId = selectedButtonId;
    }


    private async Task LoadQuestions()
    {
        var response = await SubmissionQuestionManager
            .GetAllAsync(new GetAllSubmissionQuestionQuery()
        {
            JudgingId = _judgingForm.Id,
            IncludeAnswer = true
        });

        if (response.Succeeded)
        {
            _questions = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task SubmitReferee(List<AnswerQuestionDto> answer)
    {
       
        JudgingResult.JudgingFiledAnswers = _filedRates.Select(p => new JudgingFieldAnswerDto()
        {
            Rate = p.Value,
            JudgingFiledId = p.Key
        }).ToList();

        JudgingResult.SubmitAnswerQuestions = answer;

        var response = await RefereeManager.AddRefereeResult(JudgingResult);

        if (response.Succeeded)
        {
            _snackBar.Add(Localize["Judgment submitted"], Severity.Success);
            MudDialog.Close();
        }
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
    }

    private Task ChangeRateField(int filedId, int rate)
    {
        _filedRates[filedId] = rate;
        UpdateOverallScore();
        return Task.CompletedTask;
    }

    private double _OveralScore = 0;
    private void UpdateOverallScore()
    {
        _OveralScore = _filedRates.Sum(p => p.Value);
    }
}