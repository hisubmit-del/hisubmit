using AutoMapper;
using MudBlazor;
using Hisubmit.Client.SharedModels.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using HiSubmit.Client.Infrastructure.Managers.Judgings;
using HiSubmit.Client.Infrastructure.Managers.Submissiions;
using Hisubmit.Client.SharedModels.Features.Judgings.Queries.Detail;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.AddEditJudgingButton;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.DeleteJudgiingFiiled;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.DeleteJudgingButtons;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.AddEditJudgiingButton;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.Delete;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetDetail;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using HiSubmit.Client.Pages.Festival.SubmissionQuestion;

namespace HiSubmit.Client.Pages.Festival.Judging;

public partial class Judging
{
    #region Inject

    [Inject] private IMapper Mapper { get; set; }
    [Inject] private IJudgingManager JudgingManager { get; set; }
    [Inject] private ISubmissionQuestionManager SubmissionQuestionManager { get; set; }

    #endregion

    private GetJudgingDetailResponse _judging = new();

    private GetAllSubmissionQuestionResponse _question = new();


    protected override async Task OnInitializedAsync()
    {
        await CheckPermission(Permissions.JudgingForm.View);
        await GetJudging(ProjectType.Film);
        await base.OnInitializedAsync();
    }

    private async Task AddFiled()
    {
        var parameters = new DialogParameters
        {
            {
                nameof(AddJudgingFiled.JudgingFiledModal), new AddEditJudgingFiledCommand
                {
                    JudgingId = _judging.Id
                }
            },
            { nameof(AddJudgingFiled.FestivalId), SelectedFestivalId }
        };

        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<AddJudgingFiled>(Localize["Create"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }

    private async Task DeleteFiled(int id)
    {
        var response = await JudgingManager.DeleteFiled(
            new DeleteJudgingFiledCommand
            {
                Id = id
            },
            SelectedFestivalId);
        if (response.Succeeded)
        {
            await Reset();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }


    private async Task AddButton()
    {
        var parameters = new DialogParameters
        {
            {
                nameof(AddJudgingButton.JudgingButtonModel), new AddEditJudgingButtonCommand
                {
                    JudgingId = _judging.Id
                }
            },
            { nameof(AddJudgingFiled.FestivalId), SelectedFestivalId }
        };


        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<AddJudgingButton>(Localize["Create"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }

    private async Task DeleteButton(int id)
    {
        var response =
            await JudgingManager.DeleteButton(new DeleteJudgingButtonCommand { Id = id }, SelectedFestivalId);
        if (response.Succeeded)
        {
            await Reset();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task GetJudging(ProjectType projectType)
    {
        var response = await JudgingManager.GetDetail(new GetJudgingDetailQuery()
        {
            ProjectType = projectType,
            FestivalId = SelectedFestivalId
        });
        if (response.Succeeded)
        {
            _judging = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }


    private async Task DeleteQuestion(int id)
    {
        string deleteContent = Localize["Delete Content"];
        var parameters = new DialogParameters
        {
            { nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id) }
        };
        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(Localize["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await SubmissionQuestionManager.DeleteAsync(new DeleteSubmissionQuestionCommand
            {
                Id = id,
                FestivalId = SelectedFestivalId
            });
            if (response.Succeeded)
            {
                await Reset();
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                await Reset();
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }

    private async Task AddQuestion(int id = 0)
    {
        var parameters = new DialogParameters();
        if (id != 0)
        {
            var response = await SubmissionQuestionManager
                .GetDetailAsync(new GetSubmissionQuestionDetailQuery
                {
                    FestivalId = SelectedFestivalId,
                    Id = id
                });
            if (response.Succeeded)
            {
                var model = Mapper.Map<AddEditSubmissionQuestionCommand>(response.Data);
                parameters.Add(nameof(AddEditQuestion.SubQuestionModal), model);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
        else
        {
            parameters.Add(nameof(AddEditQuestion.SubQuestionModal), new AddEditSubmissionQuestionCommand()
            {
                FestivalId = null,
                JudgingId = _judging.Id
            });
        }

        parameters.Add(nameof(AddEditQuestion.FestivalId), SelectedFestivalId);
        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog =
            _dialogService.Show<AddEditQuestion>(id == 0 ? Localize["Create"] : Localize["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
    }

    private async Task Reset()
    {
        await GetJudging(_judging.ProjectType);
        StateHasChanged();
    }

    private async Task ChangeProjectType(List<ProjectType> types)
    {
        await GetJudging(types[0]);
    }
}