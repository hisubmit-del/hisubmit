using HiSubmit.Application.Interfaces.Chat;
using HiSubmit.Application.Models.Chat;
using HiSubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HiSubmit.Server.Hubs;

public class SignalRHub : Hub
{
    public async Task OnConnectAsync(string userId)
    {
        await Clients.All.SendAsync(ApplicationConstants.SignalR.ConnectUser, userId);
    }

    public async Task OnDisconnectAsync(string userId)
    {
        await Clients.All.SendAsync(ApplicationConstants.SignalR.DisconnectUser, userId);
    }

    public async Task OnChangeRolePermissions(string userId, string roleId)
    {
        await Clients.All.SendAsync(ApplicationConstants.SignalR.LogoutUsersByRole, userId, roleId);
    }

    public async Task SendMessageAsync(ChatHistory<IChatUser> chatHistory, string userName)
    {
        await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveMessage, chatHistory, userName);
    }

    public async Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId)
    {
        await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveChatNotification, message, receiverUserId, senderUserId);
    }

    public async Task UpdateDashboardAsync()
    {
        await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveUpdateDashboard);
    }

    public async Task RegenerateTokensAsync()
    {
        await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveRegenerateTokens);
    }

    public async Task SendMessageToAdmin(int roomId)
    {
        await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveMessageAdminNotification,roomId);
        await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveMessageAdmin, roomId);
    }

    public async Task SendMessageToUser(int roomId,string userId)
    {
        await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveMessageUserNotification,userId,roomId);
        await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveMessageUser,roomId);
    }

    public async Task SendMessageToFestival(int roomId,int festivalId)
    {
        await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveMessageFestival,roomId);
        await Clients.All.SendAsync(ApplicationConstants.SignalR.ReceiveMessageFestivalNotification, festivalId,roomId);
    }
}