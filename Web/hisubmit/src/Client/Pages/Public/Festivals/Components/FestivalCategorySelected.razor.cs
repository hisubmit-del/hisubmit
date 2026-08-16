using Blazored.FluentValidation;
using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLineEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllEventCategory;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectCredits;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetProjectSpecifications;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;
using Hisubmit.Client.SharedModels.Features.Submits.Commands;
using Hisubmit.Client.SharedModels.Requests.AnswerQuestions;
using HiSubmit.Client.Infrastructure.Managers.EventCategoris;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using HiSubmit.Client.Infrastructure.Managers.Submissiions;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Services;
using HiSubmit.Client.Shared.Dialogs;

namespace HiSubmit.Client.Pages.Public.Festivals.Components;

public partial class FestivalCategorySelected
{
    #region Injection

    [Inject] public IPublicFestivalManager PublicFestivalManager { get; set; }
    [Inject] public IEventCategoryManager EventCategoryManager { get; set; }
    [Inject] public ISubmitManager SubmitManager { get; set; }
    [Inject] public IProjectManager ProjectManager { get; set; }
    [Inject] public ISubmissionQuestionManager SubmissionQuestionManager { get; set; }
    [Inject] public UserCartService UserCartService { get; set; }

    #endregion

    #region Parameter

    [Parameter] public int FestivalId { get; set; }

    [Parameter] public List<int> SelectedCategoryId { get; set; } = new();

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }

    #endregion


    private bool _processing;
    private List<GetAllSubmissionQuestionResponse> Questions { get; set; } = new();
    private List<GetAllDeadLineEventCategoryResponse> DeadLineCategories { get; set; } = new();

    private HashSet<GetAllDeadLineEventCategoryResponse> _selectedDeadLineCategories = new();

    private HashSet<GetAllDeadLineEventCategoryResponse> SelectedDeadLineCategories
    {
        get => _selectedDeadLineCategories;
        set
        {
            _selectedDeadLineCategories = value;
            CalculateSumPrice();
        }
    }

    private List<GetAllProjectResponse> Projects { get; set; }

    private Dictionary<int, FeeType> _selectedFees = new();

    private bool _loaded;


    private bool _selectCategory = true;

    private MudTable<GetAllDeadLineEventCategoryResponse> _table;
    private AddSubmitCommand Submit { get; set; } = new();

    private FluentValidationValidator _fluentValidationValidator;
    private bool Validate { get; set; } = true;
    private double _submitSumPrice = 0;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserProjects();

        Submit = new AddSubmitCommand() { FestivalId = FestivalId };
        var query = new GetAllDeadLineEventCategoryQuery()
        {
            FestivalId = FestivalId,
            Nearest = true
        };
        await LoadDeadLineCategories(query);
        await GenerateSelectedDeadlineCat();
        GenerateSelectedFees();

        await base.OnInitializedAsync();

        _loaded = true;
    }

    private Task GenerateSelectedDeadlineCat()
    {
        var selected = DeadLineCategories
            .Where(p => SelectedCategoryId.Any(id => id == p.Id) 
                        )
            .ToList();
        foreach (var s in selected)
        {
            SelectedDeadLineCategories.Add(s);
        }

        return Task.CompletedTask;
    }

    private void GenerateSelectedFees()
    {
        _selectedFees.Clear();
        foreach (var deadCategory in DeadLineCategories)
        {
            _selectedFees.Add(deadCategory.Id, deadCategory.SelectedFeeType);
        }
    }

    private async Task LoadDeadLineCategories(GetAllDeadLineEventCategoryQuery query)
    {
        var response
            = await PublicFestivalManager.GetAllGetDeadLineCategory(query);

        if (response.Succeeded)
        {
            DeadLineCategories = response.Data
                .Where(p=>p.StandardFee !=null || p.GoldFee !=null ||p.StandardFee!=null).ToList();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }

        SelectedDeadLineCategories.Clear();
    }

    private async Task LoadUserProjects()
    {
        var response = await ProjectManager.GetAllAsync(new GetAllProjectRequest
        {
            GetAllData = true,
            GetCurrentUserProjects = true
        });

        if (response.Succeeded)
        {
            Projects = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }

    private async Task TrySubmitToFestival()
    {
        var hasError = false;
        var messages = new List<string>();

        if (Submit.ProjectId == null || Submit.ProjectId == 0)
        {
            messages.Add("project not  selected");
            hasError = true;
        }

        if (!SelectedDeadLineCategories.Any())
        {
            hasError = true;
            messages.Add("No category selected");
        }

        if (hasError)
        {
            var parameters = new DialogParameters()
            {
                { nameof(ErrorItemDialog.Messages), messages }
            };
            var options = new DialogOptions()
            {
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                CloseButton = true,
                
            };
            _dialogService.Show(typeof(ErrorItemDialog), "Error Item", parameters, options);
            return;
        }

        _processing = true;
        //Submit.DeadlineEventCategoriesId = selectedFees.Where(p => p.Value != 0).Select(p => p.Key).ToList();
        Submit.DeadlineEventCategoriesId = SelectedDeadLineCategories.Select(p => p.Id)
            .ToList();
        Validate = _fluentValidationValidator.Validate(option => option.IncludeAllRuleSets());
        if (Validate)
        {
            var categoriesId = DeadLineCategories.Where(cat => Submit.DeadlineEventCategoriesId
                    .Any(id => cat.Id == id)).Select(p => p.EventCategoryId)
                .ToList();

            var questionsQuery = new GetAllSubmissionQuestionQuery
            {
                IncludeAnswer = true,
                FestivalId = FestivalId,
                CategoriesIdString = string.Join(",", categoriesId),
            };

            var responseQuestions = await SubmissionQuestionManager.GetAllAsync(questionsQuery);

            if (responseQuestions.Succeeded)
            {
                if (responseQuestions.Data.Any())
                {
                    Questions = responseQuestions.Data;
                    _selectCategory = false;
                }
                else
                {
                    await SubmitToFestival();
                }
            }
            else
            {
                foreach (var message in responseQuestions.Messages)
                {
                    _snackBar.Add(message, MudBlazor.Severity.Error);
                }
            }
        }
        else
        {
            await SubmitToFestival();
        }

        _processing = false;
    }

    private async Task SubmitToFestivalWithAnswers(List<AnswerQuestionDto> answers)
    {
        _processing = true;
        Submit.SubmitAnswerQuestions = answers;
        await SubmitToFestival();
        _processing = false;
    }

    private async Task SubmitToFestival()
    {
        Submit.DeadlineEventCategoriesId = SelectedDeadLineCategories.Select(p => p.Id)
            .ToList();

        var response = await SubmitManager.SubmitToFestival(Submit);
        if (response.Succeeded)
        {
            _snackBar.Add(Localize["Submit request  add to cart successfully"], MudBlazor.Severity.Success);
            UserCartService.ChangeUserCart();
            MudDialog.Close();
        }

        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }

    private async Task TakeProjectSpecification()
    {
        DeadLineCategories.Clear();
        if (Submit.ProjectId != null)
        {
            var query = new GetAllDeadLineEventCategoryQuery
            {
                Nearest = true,
                FestivalId = FestivalId,
                SpecfyWithProject = true,
                ProjectId = Submit.ProjectId.Value,
            };
            await LoadDeadLineCategories(query);
        }
        await GenerateSelectedDeadlineCat();
        GenerateSelectedFees();
        StateHasChanged();
    }

    private void CalculateSumPrice()
    {
        var sum = 0;
        foreach (var sdc in SelectedDeadLineCategories)
        {
            Console.WriteLine(sdc.SelectedFeeType);
            switch (sdc.SelectedFeeType)
            {
                case FeeType.Standard:
                    sum += sdc.StandardFee.Value;
                    break;
                case FeeType.Gold:
                    sum += sdc.GoldFee.Value;
                    break;
                case FeeType.Student:
                    sum += sdc.StudentFee.Value;
                    break;
            }
        }
        _submitSumPrice = sum;
    }

    private void CloseModal()
    {
       MudDialog.Cancel();
       
    }
}