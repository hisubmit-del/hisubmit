using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Chats.Commands;
using Hisubmit.Client.SharedModels.Features.Chats.Queries;
using ClientComponents.Extensions;
using HiSubmit.Client.Infrastructure.Managers.FestivalChat;
using ClientComponents.Shared.Components.Chats;
using Hisubmit.Client.SharedModels.Enums.Chats;
using HiSubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace ClientComponents.Pages.Festival.Chats;

public partial class NewChats
{
    #region Injection

    [Inject] private IFestivalChatManager ChatManager { get; set; }

    #endregion

    #region Parameter

    [CascadingParameter] public HubConnection HubConnection { get; set; }
    [Parameter]    public int? RoomId { get; set; }

    #endregion

    #region Private Filled
    private bool _openContact;
    private List<GetAllRoomResponse> _rooms = new();
    private List<GetAllContactResponse> _contacts = new();
    private List<GetAllContactResponse> _contactsAndRooms = new();
    private GetAllRoomResponse _selectedRoom;

    #endregion

    #region ChatBox Parameter

    private ChatBox _chatBox;
    private List<GetChatHistoryResponse> Messages { get; set; } = new();
    private AddChatMessageRequest _chatMessageCommand = new();
    private string SelectedRoomTitle { get; set; }
    private string SelectedRoomImageUrl { get; set; }
    private string _searchString;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadSelectedFestivalId();
        await base.OnInitializedAsync();
        if (RoomId != null && RoomId != 0)
            await LoadRoomMessages();
        
        await LoadChatRooms();
        await LoadContact();
        await GenerateContactWithRoom();
        await ConfigSignalR();
       
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
        await LoadSelectedFestivalId();
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            AccountType = SiteAccountType.Festival,
            FestivalId = SelectedFestivalId,
            NotificationTypes = NotificationType.FestivalReceivedMessage
        });
        NotificationService.ChangeNotificationBar();
    }
    #region LoadAndConfig

    private async Task ConfigSignalR()
    {
        HubConnection = HubConnection.TryInitialize(_navigationManager);
        if (HubConnection.State == HubConnectionState.Disconnected)
            await HubConnection.StartAsync();

        HubConnection.On<int>(ApplicationConstants.SignalR.ReceiveMessageFestival,
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
        await LoadSelectedFestivalId();
        var response = await ChatManager.GetAllRooms(
            new GetAllRoomQuery
            {
                FestivalId = SelectedFestivalId,
                RequestUserType = ChatRequestUserType.Festival,
            }, SelectedFestivalId);
        if (response.Succeeded)
            _rooms = response.Data;
        foreach (var message in response.Messages)
            _snackBar.Add(message, Severity.Error);

        if (RoomId != null)
            _selectedRoom = _rooms.FirstOrDefault(p => p.RoomId == RoomId);
    }

    private async Task LoadContact()
    {
        await LoadSelectedFestivalId();
        var response = await ChatManager.GetAllContact(
            new GetAllContactQuery
            {
                FestivalId = SelectedFestivalId,
                Type = ChatRequestUserType.Festival,
            }, SelectedFestivalId);
        if (response.Succeeded)
            _contacts = response.Data;
        foreach (var message in response.Messages)
            _snackBar.Add(message, Severity.Error);
    }

    private Task GenerateContactWithRoom()
    {
        foreach (var contact in _contacts.Where(p => p.ContactType != ContactType.Admin))
        {
            var room = _rooms.FirstOrDefault(p => p.UserId == contact.UserId);
            if (room != null)
            {
                contact.NotSeenCount = room.NotSeenMessageCount;
                contact.RoomId = room.RoomId;
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
                    ? ChatMessageType.FestivalToAdmin
                    : ChatMessageType.FestivalToUser;
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
        if (contact.RoomId != null)
        {
            _navigationManager.NavigateTo($"/festival/chat/{contact.RoomId}");
        }
        RoomId = contact.RoomId;
        _selectedRoom = _rooms.FirstOrDefault(p => p.RoomId == RoomId);
        var chatWithAdmin = contact.ContactType == ContactType.Admin;
        var roomType = contact.ContactType == ContactType.Admin
            ? ChatRoomType.AdminFestival
            : ChatRoomType.FestivalUser;
        var messageType = contact.ContactType == ContactType.Admin
            ? ChatMessageType.FestivalToAdmin
            : ChatMessageType.FestivalToUser;

        SelectedRoomTitle = contact.FullName;
        SelectedRoomImageUrl = contact.ImageUrl;

        if (RoomId == null)
        {
            Messages = new List<GetChatHistoryResponse>();
            RoomId = await GetRoomId(new TryGetRoomIdCommand
            {
                Type = roomType,
                FestivalId = SelectedFestivalId,
                ChatUser1 = contact.UserId,
                ChatWithAdmin = chatWithAdmin,
            });
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
                Type = ChatRequestUserType.Festival
            }, SelectedFestivalId);
            if (response.Succeeded)
                Messages = response.Data;
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
        }
    }

    private async Task<int?> GetRoomId(TryGetRoomIdCommand command)
    {
        var response = await ChatManager.GetRoomId(command, SelectedFestivalId);

        if (response.Succeeded)
            return response.Data;
        foreach (var message in response.Messages)
            _snackBar.Add(message, Severity.Error);
        return null;
    }

    private async Task AddMessage(AddChatMessageRequest command)
    {
        var response = await ChatManager.AddMessage(command, SelectedFestivalId);

        if (response.Succeeded)
        {
            await LoadRoomMessages();
            _chatMessageCommand.Text = string.Empty;
            if (command.Type == ChatMessageType.FestivalToUser)
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendMessageToUser, command.ChatRoomId,
                    _selectedRoom.UserId);
            if (command.Type == ChatMessageType.FestivalToAdmin)
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendMessageToAdmin, command.ChatRoomId);
        }

        foreach (var message in response.Messages)
            _snackBar.Add(message, Severity.Error);
    }
}