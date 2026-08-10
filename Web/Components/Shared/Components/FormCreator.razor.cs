using System;
using System.Linq;
using Hisubmit.Client.SharedModels.Enums;
using System.Threading.Tasks;
using Web.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Requests.AnswerQuestions;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;

namespace Web.Components.Shared.Components;

public partial class FormCreator
{
    #region Parameters

    [Parameter] public string Title { get; set; }
    [Parameter] public string Description { get; set; }
    [Parameter] public string SubmitButtonText { get; set; }
    [Parameter] public List<AnswerQuestionDto> AnswerQuestions { get; set; }
    [Parameter] public List<GetAllSubmissionQuestionResponse> Questions { get; set; }
    [Parameter] public EventCallback<List<AnswerQuestionDto>> AnsweredTheQuestions { get; set; }

    #endregion

    #region Private Field

    private bool _loaded;
    private Dictionary<int, int> _selectedOptions;
    private List<AnswerQuestionDto> _answerQuestion;
    private Dictionary<int, bool> _trueOrFalseOptions;
    private Dictionary<int, string> _textAndTextAreas;
    private Dictionary<int, List<CheckBoxItem<int>>> _checkedBoxOptions;

    #endregion

    protected override async Task OnInitializedAsync()
    {
        if (Questions == null)
            return;

        _trueOrFalseOptions = new Dictionary<int, bool>();
        _selectedOptions = new Dictionary<int, int>();
        _answerQuestion = new List<AnswerQuestionDto>();
        _textAndTextAreas = new Dictionary<int, string>();
        _checkedBoxOptions = new Dictionary<int, List<CheckBoxItem<int>>>();
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
                case Questiontype.TextArea:
                default:
                    await GenerateTExtAndTextArea(question);
                    break;
            }
        }
    }

    private async Task GenerateCheckBoxItem(GetAllSubmissionQuestionResponse question)
    {
        await Task.Run(() =>
        {
            _checkedBoxOptions.Add(question.Id, new List<CheckBoxItem<int>>());
            Console.WriteLine(question.Options.Count());
            foreach (var option in question.Options)
            {
                _checkedBoxOptions[question.Id].Add(new CheckBoxItem<int>()
                {
                    IsSelected = false,
                    Value = option.Id,
                    Name = option.Title
                });
            }
        });
    }

    private async Task GenerateDropDownItem(GetAllSubmissionQuestionResponse question)
    {
        await Task.Run(() => { _selectedOptions.Add(question.Id, 0); });
    }

    private async Task GenerateTrueFalse(GetAllSubmissionQuestionResponse question)
    {
        await Task.Run(() => { _trueOrFalseOptions.Add(question.Id, false); });
    }

    private async Task GenerateTExtAndTextArea(GetAllSubmissionQuestionResponse question)
    {
        await Task.Run(() => { _textAndTextAreas.Add(question.Id, string.Empty); });
    }

    private async Task SubmitForm()
    {
        foreach (var checkBoxItem in _checkedBoxOptions)
        {
            var selectedOptionIdString =
                string.Join(",", checkBoxItem.Value.Where(p => p.IsSelected).Select(p => p.Value));
            _answerQuestion.Add(new AnswerQuestionDto()
            {
                Answer = selectedOptionIdString,
                SubmissionQuestionId = checkBoxItem.Key
            });
        }

        foreach (var selectedOption in _selectedOptions)
        {
            _answerQuestion.Add(new AnswerQuestionDto()
            {
                Answer = selectedOption.Value.ToString(),
                SubmissionQuestionId = selectedOption.Key
            });
        }

        foreach (var trueFalseItem in _trueOrFalseOptions)
        {
            _answerQuestion.Add(new AnswerQuestionDto()
            {
                SubmissionQuestionId = trueFalseItem.Key,
                Answer = trueFalseItem.Value.ToString()
            });
        }

        foreach (var text in _textAndTextAreas)
        {
            _answerQuestion.Add(new AnswerQuestionDto()
            {
                SubmissionQuestionId = text.Key,
                Answer = text.Value
            });
        }

        await AnsweredTheQuestions.InvokeAsync(_answerQuestion);
    }
}