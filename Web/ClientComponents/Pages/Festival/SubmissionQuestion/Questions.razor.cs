using AutoMapper;
using Blazored.LocalStorage;
using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.Delete;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetDetail;
using ClientComponents.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Submissiions;
using HiSubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Contracts.Permission;

namespace ClientComponents.Pages.Festival.SubmissionQuestion;

public partial class Questions
{
    #region Inject

    [Inject] public IMapper Mapper { get; set; }

    [Inject] private ISubmissionQuestionManager SubmissionQuestionManager { get; set; }


    #endregion

    private string _searchString = "";
    private GetAllSubmissionQuestionResponse _question = new();
    private List<GetAllSubmissionQuestionResponse> _questionList = new();

    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        await CheckPermission(Permissions.SubmissionForm.View);
        await GetQuestionAsync();
        _loaded = true;
    }
    

    private async Task GetQuestionAsync()
    {
        var response = await SubmissionQuestionManager.GetAllAsync(new GetAllSubmissionQuestionQuery
            { FestivalId = SelectedFestivalId });
        if (response.Succeeded)
        {
            _questionList = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }

    private async Task Delete(int id)
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
            var response = await SubmissionQuestionManager.DeleteAsync(new DeleteSubmissionQuestionCommand()
                { Id = id, FestivalId = SelectedFestivalId });
            if (response.Succeeded)
            {
                await Reset();
                _snackBar.Add(response.Messages[0], MudBlazor.Severity.Success);
            }
            else
            {
                await Reset();
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, MudBlazor.Severity.Error);
                }
            }
        }
    }

    private async Task InvokeModal(int id = 0)
    {
        var parameters = new DialogParameters();
        if (id != 0)
        {
            var response = await SubmissionQuestionManager
                .GetDetailAsync(new GetSubmissionQuestionDetailQuery() { FestivalId = SelectedFestivalId, Id = id });

            if (response.Succeeded)
            {
                var model = Mapper.Map<AddEditSubmissionQuestionCommand>(response.Data);
                parameters.Add(nameof(AddEditQuestion.SubQuestionModal), model);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, MudBlazor.Severity.Error);
                }
            }
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
        _question = new GetAllSubmissionQuestionResponse();
        await GetQuestionAsync();
    }

    //private bool Search(GetAllArtCategoryyResponse brand)
    //{
    //    if (string.IsNullOrWhiteSpace(_searchString)) return true;
    //    if (brand.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
    //    {
    //        return true;
    //    }
    //    if (brand.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
    //    {
    //        return true;
    //    }
    //    return false;
    //}
}