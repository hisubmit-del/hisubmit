using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Shared.Components;

public partial class ChipsetTextField
{
    #region Parameters

    [Parameter] public string Label { get; set; }
    [Parameter] public string HelperText { get; set; }
    [Parameter] public string Value { get; set; } = " ";
    [Parameter] public char Separator { get; set; } = '-';
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    #endregion

    #region Private Field

    private List<string> _values = new();
    private string _currentString;

    #endregion

    #region Override

    protected override Task OnInitializedAsync()
    {
        if (!string.IsNullOrWhiteSpace(Value))
        {
            _values = Value.Split(Separator).ToList();
        }
        return base.OnInitializedAsync();
    }

    #endregion

    private void Remove(MudChip<string> chip)
    {
        _values.Remove(chip.Text);
    }

    private void CheckSeparator(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _currentString = string.Empty;
            return;
        }
        if (value.Last() == '-')
            AddToValue(_currentString);
        else
            _currentString = value;
    }

    private void AddCurrentToValues()
    {
        AddToValue(_currentString);
    }

    private void AddToValue(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            _values.Add(value);
            Value = string.Join(Separator, _values);
            _currentString = string.Empty;
            ValueChanged.InvokeAsync(Value);
        }
    }
}