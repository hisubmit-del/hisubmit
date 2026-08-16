using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Chats.Commands;
using Hisubmit.Client.SharedModels.Features.Chats.Queries;
using HiSubmit.Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.AdminChats;
using HiSubmit.Web.Components.Shared.Components.Chats;
using Hisubmit.Client.SharedModels.Enums.Chats;
using Hisubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.Admin.Comminucation;

public partial class NewChat
{
    #region Injection

    [Inject] private IAdminChatManager ChatManager { get; set; }

    #endregion

    #region Parameter

    [CascadingParameter] public HubConnection HubConnection { get; set; }
    [Parameter] public int? RoomId { get; set; }

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
    private string _searchString;
    private List<GetChatHistoryResponse> Messages { get; set; } = new();
    private AddChatMessageRequest _chatMessageCommand = new();
    private string SelectedRoomTitle { get; set; }
    private string SelectedRoomImageUrl { get; set; }

    private List<GetAllContactResponse> _roomContact = new();
    private List<GetAllContactResponse> _contactContact = new();

    private string SearchString
    {
        set
        {
            _roomContact = _contactsAndRooms
                .Where(p => p.RoomId != null
                            && (string.IsNullOrWhiteSpace(value)
                                || p.FullName.Contains(value)))
                .ToList();
            _contactContact = _contactsAndRooms
                .Where(p => p.RoomId == null
                            && (string.IsNullOrWhiteSpace(value)
                                || p.FullName.Contains(value)))
                .ToList();
            _searchString = value;
        }
        get => _searchString;
    }

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
            AccountType = SiteAccountType.Admin,
            NotificationTypes = NotificationType.AdminReceivedMessage
        });
        NotificationService.ChangeNotificationBar();
    }
    #region LoadAndConfig
    private async Task ConfigSignalR()
    {
        HubConnection = HubConnection.TryInitialize(_navigationManager);
        if (HubConnection.State == HubConnectionState.Disconnected)
            await HubConnection.StartAsync();

        HubConnection.On<int>(ApplicationConstants.SignalR.ReceiveMessageAdmin,
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
                RequestUserType = ChatRequestUserType.Admin,
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
            new GetAllContactQuery
            {
                Type = ChatRequestUserType.Admin,
            });
        if (response.Succeeded)
            _contacts = response.Data;
        foreach (var message in response.Messages)
            _snackBar.Add(message, Severity.Error);
    }

    private Task GenerateContactWithRoom()
    {
        foreach (var contact in _contacts)
        {
            if (contact.ContactType == ContactType.Festival)
            {
                var room = _rooms.FirstOrDefault(p => p.FestivalId == contact.FestivalId);
                if (room != null)
                {
                    contact.RoomId = room.RoomId;
                    contact.NotSeenCount = room.NotSeenMessageCount;
                }
            }

            if (contact.ContactType == ContactType.Actors)
            {
                var room = _rooms.FirstOrDefault(p => p.UserId == contact.UserId);
                if (room != null)
                {
                    contact.NotSeenCount = room.NotSeenMessageCount;
                    contact.RoomId = room.RoomId;
                }
            }

            _contactsAndRooms.Add(contact);
            _roomContact = _contactsAndRooms.Where(p => p.RoomId != null).ToList();
            _contactContact = _contactsAndRooms.Where(p => p.RoomId == null).ToList();
        }

        if (RoomId != null)
        {
            var selectedRoom = _contactsAndRooms.FirstOrDefault(p => p.RoomId == RoomId);
            if (selectedRoom != null)
            {
                SelectedRoomTitle = selectedRoom.FullName;
                SelectedRoomImageUrl = selectedRoom.ImageUrl;
                var messageType = selectedRoom.ContactType == ContactType.Festival
                    ? ChatMessageType.AdminToFestival
                    : ChatMessageType.AdminToUser;
                _chatMessageCommand = new AddChatMessageRequest()
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
            _navigationManager.NavigateTo($"/admin/chat/{contact.RoomId}");

        RoomId = contact.RoomId;
        var roomType = contact.ContactType == ContactType.Festival
            ? ChatRoomType.AdminFestival
            : ChatRoomType.AdminUser;
        var messageType = contact.ContactType == ContactType.Festival
            ? ChatMessageType.AdminToFestival
            : ChatMessageType.AdminToUser;

        SelectedRoomTitle = contact.FullName;
        SelectedRoomImageUrl = contact.ImageUrl;
        if (RoomId == null)
        {
            Messages = new List<GetChatHistoryResponse>();
            RoomId = await GetRoomId(new TryGetRoomIdCommand
            {
                Type = roomType,
                ChatWithAdmin = true,
                ChatUser1 = contact.UserId,
                FestivalId = contact.FestivalId,
            });
            if (RoomId != null)
            {
                Console.WriteLine("RoomId:null");
                _navigationManager.NavigateTo($"/admin/chat/{RoomId}");
                //contact.RoomId = RoomId;
                //_rooms.Add(new GetAllRoomResponse
                //{
                //    ItemType = roomType,
                //    RoomId = RoomId.Value,
                //    UserId = contact.UserId,
                //    Title = contact.FullName,
                //    FestivalId = contact.FestivalId,
                //});
            }
        }
        else
            await LoadRoomMessages();

        _selectedRoom = _rooms.FirstOrDefault(p => p.RoomId == RoomId);

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
                Type = ChatRequestUserType.Admin
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
        var response = await ChatManager.AddMessage(command);

        if (response.Succeeded)
        {
            await LoadRoomMessages();
            _chatMessageCommand.Text = string.Empty;
            if (command.Type == ChatMessageType.AdminToUser)
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendMessageToUser, command.ChatRoomId,
                    _selectedRoom.UserId);
            if (command.Type == ChatMessageType.AdminToFestival)
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendMessageToFestival,
                    command.ChatRoomId, _selectedRoom.FestivalId);
        }

        foreach (var message in response.Messages)
            _snackBar.Add(message, Severity.Error);
    }
}