using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Reviews.Commands;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Web.Components.Shared.Components;

public partial class ReviewDialog
{
    #region Injection

    [Inject] private ISubmitManager SubmitManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int FestivalId { get; set; }
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }
    [Parameter] public CommentType Type { get; set; }

    #endregion

    #region Private Filled

    private AddReviewCommand _model = new();
    private bool _validated = true;
    private FluentValidationValidator _fluentValidationValidator;
    private bool _processing;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        _model.FestivalId = FestivalId;
        await base.OnInitializedAsync();
    }

    #endregion

    private async Task SaveAsync()
    {
        _processing = true;
        _validated = _fluentValidationValidator
            .Validate(param => param.IncludeAllRuleSets());
        if (_validated)
        {
            _model.Type = Type;
            _model.FestivalId = FestivalId;
            var response = await SubmitManager.Review(_model);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
        }
        _processing = false;
    }

    private void Cancel()
    {
       MudDialog.Close();
    }
}
