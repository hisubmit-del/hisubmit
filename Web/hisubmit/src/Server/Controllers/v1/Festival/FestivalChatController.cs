using System;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Chats.Commands;
using HiSubmit.Application.Features.Chats.Queries;
using HiSubmit.Application.Interfaces.Chat;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Models.Chat;
using Microsoft.AspNetCore.Mvc;

namespace HiSubmit.Server.Controllers.v1.Festival;

public class FestivalChatController :BaseFestivalController<FestivalChatController>
{
    private readonly ICurrentUserService _currentUserService;
          private readonly IChatService _chatService;
  
          public FestivalChatController(ICurrentUserService currentUserService, IChatService chatService)
          {
              _currentUserService = currentUserService;
              _chatService = chatService;
          }
  
          /// <summary>
          /// Get user wise chat history
          /// </summary>
          /// <param name="contactId"></param>
          /// <returns>Enable 200 OK</returns>
          //Get user wise chat history
          [HttpGet("{contactId}")]
          public async Task<IActionResult> GetChatHistoryAsync(string contactId, int festivalId)
          {
              return Ok(await _chatService.GetChatHistoryAsync(_currentUserService.UserId, contactId,false,festivalId));
          }
          /// <summary>
          /// get available users
          /// </summary>
          /// <returns>Enable 200 OK</returns>
          //get available users - sorted by date of last message if exists
          [HttpGet("users")]
          public async Task<IActionResult> GetChatUsersAsync(int festivalId)
          {
              return Ok(await _chatService.GetFestivalChatUserAsync(festivalId));
          }

          /// <summary>
          /// Save Chat Message
          /// </summary>
          /// <param name="message"></param>
          /// <param name="festivalId"></param>
          /// <returns>Enable 200 OK</returns>
          //save chat message
          [HttpPost]
          public async Task<IActionResult> SaveMessageAsync(ChatHistory<IChatUser> message, int festivalId)
          {
              message.FromFestivalId = festivalId;
              message.FromUserId = _currentUserService.UserId;
              message.ToUserId = message.ToUserId;
              message.CreatedDate = DateTime.Now;
              return Ok(await _chatService.SaveMessageAsync(message));
          }
          
          
          [HttpGet("Rooms")]
          public async Task<IActionResult> GetAllRooms([FromQuery] GetAllRoomQuery query,int festivalId)
          {
              // request.UserId = _currentUserService.UserId;
              query.FestivalId = festivalId;
              query.RequestUserType = ChatRequestUserType.Festival;
              return Ok(await Mediator.Send(query));
          }

          [HttpGet("ChatMessages")]
          public async Task<IActionResult> GetAllMessages([FromQuery] GetChatHistoryQuery query)
          {
              return Ok(await Mediator.Send(query));
          }

          [HttpPost("AddMessage")]
          public async Task<IActionResult> AddMessage(AddChatMessageCommand command)
          {
              return Ok(await  Mediator.Send(command));
          }

          [HttpGet("Contacts")]
          public async Task<IActionResult> AllContacts([FromQuery]GetAllContactQuery query,int festivalId)
          {
              query.FestivalId = festivalId;
              query.Type = ChatRequestUserType.Festival;
              return Ok(await Mediator.Send(query));
          }

          [HttpPost("GetRoomId")]
          public async Task<IActionResult> GetRoomId(TryGetRoomIdCommand command,int festivalId)
          {
              command.FestivalId = festivalId;
              return Ok(await  Mediator.Send(command));
          }
}