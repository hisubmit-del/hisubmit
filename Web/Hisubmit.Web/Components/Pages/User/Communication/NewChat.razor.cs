using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Chats.Commands;
using Hisubmit.Client.SharedModels.Features.Chats.Queries;
using HiSubmit.Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Communication;
using HiSubmit.Web.Components.Shared.Components.Chats;
using Hisubmit.Client.SharedModels.Enums.Chats;
using Hisubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.User.Communication;

public partial class NewChat
{
    #region Injection

    [Inject] private IChatManager ChatManager { get; set; }

    #endregion

    #region Parameter

    [Parameter] public int? RoomId { get; set; }
    [CascadingParameter] public HubConnection HubConnection { get; set; }

    #endregion

    #region Private Filled

    private bool _openContact;
    private GetAllRoomResponse _selectedRoom;
    private List<GetAllRoomResponse> _rooms = new();
    private List<GetAllContactResponse> _contacts = new();
    private List<GetAllContactResponse> _contactsAndRooms = new();

    #endregion

    #region ChatBox Parameter

    private ChatBox _chatBox;
    private List<GetChatHistoryResponse> Messages { get; set; } = new();
    private AddChatMessageRequest _chatMessageCommand = new();
    private string SelectedRoomTitle { get; set; }
    private string SelectedRoomImageUrl { get; set; }

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        if (RoomId != null && RoomId != 0)
        {
            await LoadRoomMessages();
        }

        await LoadChatRooms();
        await LoadContact();
        await GenerateContactWithRoom();
        await ConfigSignalR();
        await base.OnInitializedAsync();
    }
 
     protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SeenNotification();
        }

        await base.OnAfterRenderAsync(firstRender);
    }
    #endregion
    private async Task SeenNotification()
    {
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            AccountType = SiteAccountType.User,
            NotificationTypes = NotificationType.UserReceivedMessage,
            UserId =(await AuthenticationManager.CurrentUser()).GetUserId()
        });
        NotificationService.ChangeNotificationBar();
    }
  
   
    #region LoadAndConfig

    private async Task ConfigSignalR()
    {
        HubConnection = HubConnection.TryInitialize(_navigationManager);
        if (HubConnection.State == HubConnectionState.Disconnected)
            await HubConnection.StartAsync();

        HubConnection.On<int>(ApplicationConstants.SignalR.ReceiveMessageUser,
            async (roomId) =>
            {
                if (roomId == RoomId)
                {
                    await LoadRoomMessages();
                    StateHasChanged();
                }
            });
    }

    private async Task LoadChatRooms()
    {
        var response = await ChatManager.GetAllRooms(
            new GetAllRoomQuery
            {
                RequestUserType = ChatRequestUserType.User,
                UserId = (await AuthenticationManager.CurrentUser()).GetUserId(),
            });
        if (response.Succeeded)
            _rooms = response.Data;
        foreach (var message in response.Messages)
            _snackBar.Add(message, Severity.Error);

        if (RoomId != null)
            _selectedRoom = _rooms.FirstOrDefault(p => p.RoomId == RoomId);
    }

    private async Task LoadContact()
    {
        var response = await ChatManager.GetAllContact(
            new GetAllContactQuery()
            {
                Type = ChatRequestUserType.User,
                UserId = (await AuthenticationManager.CurrentUser()).GetUserId(),
            });
        if (response.Succeeded)
            _contacts = response.Data;
        foreach (var message in response.Messages)
            _snackBar.Add(message, Severity.Error);
    }

    private Task GenerateContactWithRoom()
    {
        foreach (var contact in _contacts.Where(p => p.ContactType != ContactType.Admin))
        {
            var room = _rooms.FirstOrDefault(p => p.FestivalId == contact.FestivalId);
            if (room != null)
            {
                contact.RoomId = room.RoomId;
                contact.NotSeenCount = room.NotSeenMessageCount;
            }

            _contactsAndRooms.Add(contact);
        }

        var adminContact = _contacts.FirstOrDefault(p => p.ContactType == ContactType.Admin);
        var adminRoom = _rooms.FirstOrDefault(p => p.Type == ChatRoomType.AdminUser);
        if (adminRoom != null && adminContact != null)
        {
            adminContact.RoomId = adminRoom.RoomId;
            adminContact.NotSeenCount = adminRoom.NotSeenMessageCount;
        }

        _contactsAndRooms.Add(adminContact);
        if (RoomId != null)
        {
            var selectedRoom = _contactsAndRooms.FirstOrDefault(p => p.RoomId == RoomId);
            if (selectedRoom !=null)
            {
                SelectedRoomTitle = selectedRoom.FullName;
                SelectedRoomImageUrl = selectedRoom.ImageUrl;
                var messageType = selectedRoom.ContactType == ContactType.Admin
                    ? ChatMessageType.UserToAdmin
                    : ChatMessageType.UserToFestival;
                _chatMessageCommand = new AddChatMessageRequest
                {
                    Type = messageType,
                    ChatRoomId = RoomId.Value,
                };
            }
        }
        return Task.CompletedTask;
    }
    #endregion


    private async Task LoadUserChat(GetAllContactResponse contact)
    {
        _openContact = false;
        if (contact.RoomId != null)
            _navigationManager.NavigateTo($"/chat/{contact.RoomId}");

        RoomId = contact.RoomId;
        _selectedRoom = _rooms.FirstOrDefault(p => p.RoomId == RoomId);
        var chatWithAdmin = contact.ContactType == ContactType.Admin;
        var roomType = contact.ContactType == ContactType.Admin ? ChatRoomType.AdminUser : ChatRoomType.FestivalUser;
        var messageType = contact.ContactType == ContactType.Admin
            ? ChatMessageType.UserToAdmin
            : ChatMessageType.UserToFestival;

        SelectedRoomTitle = contact.FullName;
        SelectedRoomImageUrl = contact.ImageUrl;
        if (RoomId == null)
        {
            Messages = new List<GetChatHistoryResponse>();
            RoomId = await GetRoomId(new TryGetRoomIdCommand
            {
                Type = roomType,
                ChatWithAdmin = chatWithAdmin,
                FestivalId = contact.FestivalId,
            });
            _navigationManager.NavigateTo($"/chat/{RoomId}");
            contact.RoomId = RoomId;
        }
        else
            await LoadRoomMessages();

        if (RoomId != null)
        {
            _chatMessageCommand = new AddChatMessageRequest
            {
                Type = messageType,
                ChatRoomId = RoomId.Value,
            };
        }
    }

    private async Task LoadRoomMessages()
    {
        if (RoomId != null)
        {
            var response = await ChatManager.GetAllChatMessage(new GetChatHistoryQuery
            {
                RoomId = RoomId.Value,
                Type = ChatRequestUserType.User
            });
            if (response.Succeeded)
                Messages = response.Data;
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
        }
    }

    private async Task<int?> GetRoomId(TryGetRoomIdCommand command)
    {
        var response = await ChatManager.GetRoomId(command);

        if (response.Succeeded)
            return response.Data;
        foreach (var message in response.Messages)
            _snackBar.Add(message, Severity.Error);
        return null;
    }


    private async Task AddMessage(AddChatMessageRequest command)
    {
        Console.WriteLine(command.ChatRoomId + " " + command.Text + " " + command.Type);
        var response = await ChatManager.AddMessage(command);
        if (response.Succeeded)
        {
            await LoadRoomMessages();
            _chatMessageCommand.Text = string.Empty;
            if (command.Type == ChatMessageType.UserToAdmin)
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendMessageToAdmin, command.ChatRoomId);
            if (command.Type == ChatMessageType.UserToFestival)
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendMessageToFestival,
                    command.ChatRoomId, _selectedRoom.FestivalId);
        }

        foreach (var message in response.Messages)
            _snackBar.Add(message, Severity.Error);
    }
}