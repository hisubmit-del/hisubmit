using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Chats.Commands;
using Hisubmit.Client.SharedModels.Features.Chats.Queries;
using HiSubmit.Client.Infrastructure.Managers.Communication;
using Hisubmit.Client.SharedModels.Enums.Chats;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace Web.Components.Shared.Components.Chats;

public partial class ChatBox
{
    #region Parameter

    [Parameter]
    public  string SendMessagePolicy { get; set; }
    [Parameter] public int? RoomId { get; set; }

    [Parameter] public List<GetChatHistoryResponse> Messages { get; set; }

    [Parameter] public EventCallback<AddChatMessageRequest> OnSendMessage { get; set; }

    [Parameter] public AddChatMessageRequest ChatMessage { get; set; }

    [Parameter] public string CFullName { get; set; }
    [Parameter] public string ImageUrl { get; set; }
    [Parameter]public bool OpenContact { get; set; }
    [Parameter]public EventCallback<bool> OpenContactChanged { get; set; }

    #endregion


    #region Override

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer2");
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    #endregion
    private async Task SubmitAsync()
    {
        if (string.IsNullOrWhiteSpace(ChatMessage.Text))
        {
            _snackBar.Add("Text is Required", Severity.Warning);
        }
        else
        {
            await OnSendMessage.InvokeAsync(ChatMessage);
            await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer2");
            ChatMessage.Text = string.Empty;
        }
    }

    private async Task OnKeyPressInChat(KeyboardEventArgs e)
    {
        if (e.Code is "Enter" or "NumpadEnter")
            await SubmitAsync();

    }

    private async Task OpenDrawer()
    {
        OpenContact = true;
        await OpenContactChanged.InvokeAsync(OpenContact);
    }

    public async Task HasNewMessage()
    {
        await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer2");
    }
}