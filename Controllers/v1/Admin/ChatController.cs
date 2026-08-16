using System;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Chats.Commands;
using HiSubmit.Application.Features.Chats.Queries;
using HiSubmit.Application.Interfaces.Chat;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Models.Chat;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Admin;

public class ChatController : BaseAdminController<ChatController>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IChatService _chatService;

    public ChatController(ICurrentUserService currentUserService, IChatService chatService)
    {
        _currentUserService = currentUserService;
        _chatService = chatService;
    }

    
    /// <summary>
    /// Get user wise chat history
    /// </summary>
    /// <param name="contactId"></param>
    /// <param name="festivalId"></param>
    /// <returns>Enable 200 OK</returns>
    //Get user wise chat history
    [HttpGet("history")]
    [Authorize(Policy = Permissions.AdminChat.View)]
    public async Task<IActionResult> GetChatHistoryAsync(string contactId, int? festivalId)
    {
        return Ok(await _chatService.GetChatHistoryAsync(_currentUserService.UserId, contactId, true, festivalId));
    }

    /// <summary>
    /// get available users
    /// </summary>
    /// <returns>Enable 200 OK</returns>
    //get available users - sorted by date of last message if exists
    [HttpGet("users")]
    [Authorize(Policy = Permissions.AdminChat.View)]
    public async Task<IActionResult> GetChatUsersAsync()
    {
        var currentUserId = _currentUserService.UserId;
        return Ok(await _chatService.GetAdminChatUsersAsync(currentUserId));
    }

    /// <summary>
    /// Save Chat Message
    /// </summary>
    /// <param name="message"></param>
    /// <returns>Enable 200 OK</returns>
    //save chat message
    [HttpPost("save")]
    [Authorize(Policy = Permissions.AdminChat.SendMessage)]
    public async Task<IActionResult> SaveMessageAsync(ChatHistory<IChatUser> message)
    {
        message.AdminSender = true;
        message.FromUserId = _currentUserService.UserId;
        message.ToUserId = message.ToUserId;
        message.CreatedDate = DateTime.Now;
        return Ok(await _chatService.SaveMessageAsync(message));
    }
    
    
    [HttpGet("Rooms")]
    [Authorize(Policy = Permissions.AdminChat.View)]
    public async Task<IActionResult> GetAllRooms([FromQuery] GetAllRoomQuery query)
    {
        // request.UserId = _currentUserService.UserId;
        query.RequestUserType = ChatRequestUserType.Admin;
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("ChatMessages")]
    [Authorize(Policy = Permissions.AdminChat.View)]
    public async Task<IActionResult> GetAllMessages([FromQuery] GetChatHistoryQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpPost("AddMessage")]
    [Authorize(Policy = Permissions.AdminChat.SendMessage)]
    public async Task<IActionResult> AddMessage(AddChatMessageCommand command)
    {
        return Ok(await  Mediator.Send(command));
    }

    [HttpGet("Contacts")]
    [Authorize(Policy = Permissions.AdminChat.View)]
    public async Task<IActionResult> AllContacts([FromQuery]GetAllContactQuery query)
    {
        query.UserId = _currentUserService.UserId;
        query.Type = ChatRequestUserType.Admin;
        return Ok(await Mediator.Send(query));
    }

    [HttpPost("GetRoomId")]
    [Authorize(Policy = Permissions.AdminChat.View)]
    public async Task<IActionResult> GetRoomId(TryGetRoomIdCommand command)
    {
        return Ok(await  Mediator.Send(command));
    }
}
