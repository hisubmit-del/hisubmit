using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminDashboard.Wasm.Models;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;
using Hisubmit.Client.SharedModels.Requests.AnswerQuestions;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;

namespace HiSubmit.Web.Components.Shared.Components;

public partial class FormCreatorWithAnswer
{
    [Parameter] public List<GetAllSubmissionQuestionResponse> Questions { get; set; }
    private List<AnswerQuestionDto> AnswerQuestion { get; set; }
    [Parameter] public EventCallback<List<AnswerQuestionDto>> AnsweredTheQuestions { get; set; }
    [Parameter] public string SubmitButtonText { get; set; }

    [Parameter] public List<AnswerQuestionDto> AnswerQuestions { get; set; } = new();

    [Parameter] public bool ReadOnlyMood { get; set; } = true;
    
    [Parameter]public bool HasHeader { get; set; }
    [Parameter]public RenderFragment HeaderContent { get; set; }

    private bool _loaded;
    private Dictionary<int, List<CheckBoxItem<int>>> CheckedBoxOptions { get; set; }
    private Dictionary<int, int> SelectedOptions { get; set; }
    private Dictionary<int, bool> TrueOrFalseDictionary { get; set; }
    private Dictionary<int, string> TextAndTextAreas { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Questions == null)
        {
            return;
        }

        CheckedBoxOptions = new Dictionary<int, List<CheckBoxItem<int>>>();
        SelectedOptions = new Dictionary<int, int>();
        TrueOrFalseDictionary = new Dictionary<int, bool>();
        TextAndTextAreas = new Dictionary<int, string>();
        AnswerQuestion = new List<AnswerQuestionDto>();

        await GenerateDictionaries();
        await base.OnInitializedAsync();
        _loaded = true;

    }

    private async Task GenerateDictionaries()
    {
        foreach (var question in Questions)
        {
            switch (question.Questiontype)
            {
                case Questiontype.CheckBox:
                    await GenerateCheckBoxItem(question);
                    break;
                case Questiontype.DropDownMenu:
                    await GenerateDropDownItem(question);
                    break;
                case Questiontype.True_False:
                    await GenerateTrueFalse(question);
                    break;
                case Questiontype.Text:
                    await GenerateTExtAndTextArea(question);
                    break;
                case  Questiontype.TextArea:
                    await GenerateTExtAndTextArea(question);
                    break;
            }
        }
    }

    private async Task GenerateCheckBoxItem(GetAllSubmissionQuestionResponse question)
    {
        await Task.Run(() =>
        {

            var answer = AnswerQuestions.FirstOrDefault(p => p.SubmissionQuestionId == question.Id);
            var selectedOption = new List<int>();
            if (answer != null)
            {
                selectedOption = answer.Answer.Split(',').Select(int.Parse).ToList();
            }

            CheckedBoxOptions.Add(question.Id, new List<CheckBoxItem<int>>());

            foreach (var option in question.Options)
            {
                var isSelected = selectedOption.Any(p => p == option.Id);
                CheckedBoxOptions[question.Id].Add(new CheckBoxItem<int>()
                {
                    IsSelected = isSelected,
                    Value = option.Id,
                    Name = option.Title
                });
            }
        });
    }

    private async Task GenerateDropDownItem(GetAllSubmissionQuestionResponse question)
    {
        var answer = AnswerQuestions.FirstOrDefault(p => p.SubmissionQuestionId == question.Id);
        var selected = 0;
        if (answer != null)
        {
            selected = int.Parse(answer.Answer);
        }

        await Task.Run(() => { SelectedOptions.Add(question.Id, selected); });
    }

    private async Task GenerateTrueFalse(GetAllSubmissionQuestionResponse question)
    {
        var answer = AnswerQuestions.FirstOrDefault(p => p.SubmissionQuestionId == question.Id);
        var selected = false;
        if (answer != null)
        {
            selected = bool.Parse(answer.Answer);
        }

        await Task.Run(() => { TrueOrFalseDictionary.Add(question.Id, selected); });
    }

    private async Task GenerateTExtAndTextArea(GetAllSubmissionQuestionResponse question)
    {
        var answer = AnswerQuestions.FirstOrDefault(p => p.SubmissionQuestionId == question.Id);
        var text = string.Empty;
        if (answer != null)
        {
            text = answer.Answer;
        }

        await Task.Run(() => { TextAndTextAreas.Add(question.Id, text); });
    }

    private async Task SubmitForm()
    {
        foreach (var checkBoxItem in CheckedBoxOptions)
        {
            var selectedOptionIdString =
                string.Join(",", checkBoxItem.Value.Where(p => p.IsSelected).Select(p => p.Value));
            AnswerQuestion.Add(new AnswerQuestionDto()
            {
                Answer = selectedOptionIdString,
                SubmissionQuestionId = checkBoxItem.Key
            });
        }

        foreach (var selectedOption in SelectedOptions)
        {
            AnswerQuestion.Add(new AnswerQuestionDto()
            {
                Answer = selectedOption.Value.ToString(),
                SubmissionQuestionId = selectedOption.Key
            });
        }

        foreach (var trueFalseItem in TrueOrFalseDictionary)
        {
            AnswerQuestion.Add(new AnswerQuestionDto()
            {
                SubmissionQuestionId = trueFalseItem.Key,
                Answer = trueFalseItem.Value.ToString()
            });
        }

        foreach (var text in TextAndTextAreas)
        {
            AnswerQuestion.Add(new AnswerQuestionDto()
            {
                SubmissionQuestionId = text.Key,
                Answer = text.Value
            });
        }

        await AnsweredTheQuestions.InvokeAsync(AnswerQuestion);
    }
}

