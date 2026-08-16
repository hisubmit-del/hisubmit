using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace ClientComponents.Shared.Components;

public  partial class ReadMore
{
    [Parameter]
    public string Text { get; set; }
    
    [Parameter]
    public bool MarkUpString { get; set; }

    private bool _showMore;

    private string _displayText;

    protected override Task OnInitializedAsync()
    {
        if (Text.Length > 400)
            _displayText = _showMore ? Text : Text[..400];       
        else
            _displayText = Text;
        return base.OnInitializedAsync();
    }

    private void ToggleShow()
    {
        _showMore =! _showMore;
        ToggleText();
    }

    private void ToggleText()
    {
        if (Text.Length > 250)
            _displayText = _showMore ? Text : Text[..250];       
        else
            _displayText = Text;
    }
}
