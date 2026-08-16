using Web.Models;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllEventCategory;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.EventCategoris;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using HiSubmit.Client.Infrastructure.Managers.Submissiions;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Components.Pages.Festival.SubmissionQuestion
{
    public partial class AddEditQuestion
    {
        [Inject] private ISubmissionQuestionManager SubmissionManager { get; set; }
        [Inject] private IEventCategoryManager EventCategoryManager { get; set; }

        [Parameter]
        public int FestivalId { get; set; }
        [Parameter]
        public AddEditSubmissionQuestionCommand SubQuestionModal { get; set; } = new()
        {
            Questiontype = Questiontype.Text,
            EventCategoriesId = new List<int>(),
            Options = new List<UpdateDropDownCheckBoxOption>(),
            ApplyforAllCategory = true
        };
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

        private List<CheckBoxItem<int>> CategorirsCheckBox { get; set; } = new();
        private List<GetAllEventCategoryResponse> Categories { get; set; } = new();

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated { get; set; } = true;

        bool Loaded = false;
        public void Cancel()
        {
            MudDialog.Cancel();
        }


        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            Loaded = true;
        }

        private async Task LoadDataAsync()
        {
            await GetCategories();
            await GenerateCheckBoxItem();
        }


        private async Task GetCategories()
        {
            var response = await EventCategoryManager.GetAllAsync(new GetAllEventCategoryQuery { FestivalId = FestivalId });
            if (response.Succeeded)
            {
                Categories = response.Data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private bool _processing;

        private async Task SaveAsync()
        {
            _processing = true;
            Validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
            SubQuestionModal.FestivalId = FestivalId;
            SubQuestionModal.EventCategoriesId = CategorirsCheckBox.Where(p => p.IsSelected).Select(p => p.Value).ToList();
            var response = await SubmissionManager.UpdateAsync(SubQuestionModal,FestivalId);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            _processing = false;
        }
        private async Task AddOption()
        {
            await Task.Run(() =>
            {
                SubQuestionModal.Options.Add(new UpdateDropDownCheckBoxOption());
            });
        }

        private async Task deleteOption(int id)
        {
            var option = SubQuestionModal.Options.FirstOrDefault(p => p.Id == id);
            SubQuestionModal.Options.Remove(option);
        }

        private async Task GenerateCheckBoxItem()
        {
            await Task.Run(() =>
            {
                foreach (var cat in Categories)
                {
                    var selected = SubQuestionModal.EventCategoriesId.Any(p => p == cat.Id);
                    CategorirsCheckBox.Add(new CheckBoxItem<int>
                    {
                        IsSelected = selected,
                        Name = cat.Name,
                        Value = cat.Id
                    });
                }
            });
        }
    }
}
