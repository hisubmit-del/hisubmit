using System;
using System.Threading.Tasks;
using ClientComponents.Pages.Festival.FestivalEditComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ClientComponents.Shared.Components;

public partial class CustomeRichTextEditor
{
    [Inject] private IJSRuntime Js { get; set; }

    private string BodyText { get; set; }

    [Parameter] public string ElementId { get; set; } = "editor";

    [Parameter] public string Body { get; set; }

    [Parameter] public bool SimpleOption { get; set; }
    [Parameter] public string MinHeight { get; set; } = "120px";
    [Parameter] public bool EnableLink { get; set; } = true;

    [Parameter] public EventCallback<string> BodyChanged { get; set; }

    private DotNetObjectReference<CustomeRichTextEditor> _ref;


    protected override async Task OnInitializedAsync()
    {
        await SetDotNetReference();
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Js.InvokeVoidAsync("createRichText", ElementId, SimpleOption, Body,EnableLink);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable("ChangedRicheText")]
    public async Task ChangedRicheText(string content)
    {
        Body = content;
        await BodyChanged.InvokeAsync(Body);
    }


    public async Task<string> GetContent()
    {
        await Js.InvokeVoidAsync("RemoveRichTextEvent", ElementId);
        _ref.Dispose();
        return BodyText;
    }

    private async Task SetDotNetReference()
    {
        _ref = DotNetObjectReference.Create(this);
        await Js.InvokeVoidAsync("GLOBALB.SetDotnetReference", _ref, ElementId);
    }
}