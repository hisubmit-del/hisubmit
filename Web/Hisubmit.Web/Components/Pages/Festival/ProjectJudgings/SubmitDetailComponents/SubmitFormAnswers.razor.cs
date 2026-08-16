using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetSubmitFormAnswers;
using Hisubmit.Client.SharedModels.Requests.AnswerQuestions;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubmit;
using HiSubmit.Client.Infrastructure.Managers.Submissiions;
using Microsoft.AspNetCore.Components;

namespace HiSubmit.Web.Components.Pages.Festival.ProjectJudgings.SubmitDetailComponents;

public partial class SubmitFormAnswers
{
    #region Injects

    [Inject] private IFestivalSubmitManager FestivalSubmitManager { get; set; }
    [Inject] private ISubmissionQuestionManager SubmissionQuestionManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public List<int> CategoriesId { get; set; }
    [Parameter] public int FestivalId { get; set; }
    [Parameter] public int SubmitId { get; set; }

    #endregion

    #region Private Field

    private List<GetAllSubmissionQuestionResponse> _questions;
    private List<AnswerQuestionDto> _answers;
    private bool _loaded;

    #endregion

    protected override async Task OnInitializedAsync()
    {
        await LoadSubmitQuestion();
        await LoadSubmissionFormAnswers();
        await base.OnInitializedAsync();
        _loaded = true;
    }

    private async Task LoadSubmitQuestion()
    {
        var response = await SubmissionQuestionManager.GetAllAsync(new GetAllSubmissionQuestionQuery
        {
            IncludeAnswer = true,
            FestivalId = FestivalId,
            CategoriesIdString = string.Join(',', CategoriesId),
        });
        if (response.Succeeded)
            _questions = response.Data;
    }

    private async Task LoadSubmissionFormAnswers()
    {
        var response = await FestivalSubmitManager.GetSubmitFormAnswers(new GetSubmitFormAnswersQuery
        {
            SubmitId = SubmitId,
            FestivalId = FestivalId,
        });
        if (response.Succeeded)
            _answers = response.Data;
    }
}
