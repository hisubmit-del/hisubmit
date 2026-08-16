using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Web.Components.Shared.Components;

public partial class ReadMore
{
    private const int CollapseThreshold = 420;

    [Parameter]
    public string Text { get; set; } = string.Empty;

    [Parameter]
    public bool MarkUpString { get; set; }

    private bool _showMore;
    private string _text = string.Empty;

    private bool IsExpandable => _text.Length > CollapseThreshold;

    protected override Task OnParametersSetAsync()
    {
        _text = Text ?? string.Empty;
        return Task.CompletedTask;
    }

    private void ToggleShow()
    {
        _showMore = !_showMore;
    }
}
