using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Shared.Components;

public partial class LoadingButton:MudButton
{
    [Parameter] public bool Processing { get; set; } = false;
    [Parameter] public string ProcessingTitle { get; set; } = "Processing";
    [Parameter] public Color ProcessingColor { get; set; } = Color.Default;

    private MudButton _button ;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (!string.IsNullOrWhiteSpace(EndIcon))
        {
            _button.EndIcon = EndIcon;
        }

        if (!string.IsNullOrWhiteSpace(StartIcon))
        {
            _button.StartIcon = StartIcon;
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnClickEvent()
    {
        await OnClick.InvokeAsync();
    }
}