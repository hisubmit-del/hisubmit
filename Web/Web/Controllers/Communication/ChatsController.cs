using HiSubmit.Application.Interfaces.Chat;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Models.Chat;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Chats.Commands;
using HiSubmit.Application.Features.Chats.Queries;
using MediatR;

namespace Web.Controllers.Communication;

// [Authorize(Policy = Permissions.Communication.Chat)]
[Route("api/[controller]")]
[ApiController]
public class ChatsController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IChatService _chatService;
    private readonly IMediator _mediator;

    public ChatsController
    (ICurrentUserService currentUserService,
        IChatService chatService, IMediator mediator)
    {
        _currentUserService = currentUserService;
        _chatService = chatService;
        _mediator = mediator;
    }

    /// <summary>
    /// Get user wise chat history
    /// </summary>
    /// <param name="contactId"></param>
    /// <returns>Enable 200 OK</returns>
    //Get user wise chat history
    [HttpGet("chatHistory")]
    public async Task<IActionResult> GetChatHistoryAsync(string contactId, int? festivalId, bool fsa)
    {
        contactId = _currentUserService.UserId;
        return Ok(await _chatService.GetChatHistoryAsync(_currentUserService.UserId, contactId, fsa, festivalId));
    }

    /// <summary>
    /// get available users
    /// </summary>
    /// <returns>Enable 200 OK</returns>
    //get available users - sorted by date of last message if exists
    [HttpGet("users")]
    public async Task<IActionResult> GetChatUsersAsync()
    {
        return Ok(await _chatService.GetUserChatUsersAsync(_currentUserService.UserId));
    }

    /// <summary>
    /// Save Chat Message
    /// </summary>
    /// <param name="message"></param>
    /// <returns>Enable 200 OK</returns>
    //save chat message
    [HttpPost]
    public async Task<IActionResult> SaveMessageAsync(ChatHistory<IChatUser> message)
    {
        message.FromUserId = _currentUserService.UserId;
        // message.ToUserId = message.ToUserId;
        message.CreatedDate = DateTime.Now;
        if (message.ToFestivalId != 0)
        {
            message.ToUserId = null;
        }

        return Ok(await _chatService.SaveMessageAsync(message));
    }


    [HttpGet("Rooms")]
    public async Task<IActionResult> GetAllRooms([FromQuery] GetAllRoomQuery query)
    {
       // request.UserId = _currentUserService.UserId;
        query.RequestUserType = ChatRequestUserType.User;
        return Ok(await _mediator.Send(query));
    }

    [HttpGet("ChatMessages")]
    public async Task<IActionResult> GetAllMessages([FromQuery] GetChatHistoryQuery query)
    {
        return Ok(await _mediator.Send(query));
    }

    [HttpPost("AddMessage")]
    public async Task<IActionResult> AddMessage(AddChatMessageCommand command)
    {
        return Ok(await  _mediator.Send(command));
    }

    [HttpGet("Contacts")]
    public async Task<IActionResult> AllContacts([FromQuery]GetAllContactQuery query)
    {
        query.UserId = _currentUserService.UserId;
        query.Type = ChatRequestUserType.User;
        return Ok(await _mediator.Send(query));
    }

    [HttpPost("GetRoomId")]
    public async Task<IActionResult> GetRoomId(TryGetRoomIdCommand command)
    {
        command.ChatUser1 = _currentUserService.UserId;
        return Ok(await  _mediator.Send(command));
    }
}
