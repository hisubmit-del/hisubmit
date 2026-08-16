using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Domain.Entities.Chats;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums.Chats;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Chats.Queries;

public class GetAllRoomQuery : IRequest<IResult<List<GetAllRoomResponse>>>
{
    public int? FestivalId { get; set; }
    public string UserId { get; set; }
    public ChatRequestUserType RequestUserType { get; set; }
}

public class GetAllRoomQueryHandler : IRequestHandler<GetAllRoomQuery, IResult<List<GetAllRoomResponse>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IUserService _userService;

    public GetAllRoomQueryHandler(IUnitOfWork<int> unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<IResult<List<GetAllRoomResponse>>> Handle(GetAllRoomQuery request,
        CancellationToken cancellationToken)
    {
        List<GetAllRoomResponse> responses = new();
        List<ChatRoom> rooms;
        switch (request.RequestUserType)
        {
            case ChatRequestUserType.User:
                rooms = await _unitOfWork.Repository<ChatRoom>()
                    .Entities
                   .Where(p => p.ChatUser1 == request.UserId )
                    .ToListAsync(cancellationToken);
                break;
            case ChatRequestUserType.Admin:
                rooms = await _unitOfWork.Repository<ChatRoom>()
                    .Entities
                    .Where(p => p.Type == ChatRoomType.AdminFestival || p.Type == ChatRoomType.AdminUser)
                    .ToListAsync(cancellationToken);
                break;
            case ChatRequestUserType.Festival:
                rooms = await _unitOfWork.Repository<ChatRoom>()
                    .Entities
                    .Where(p => p.FestivalId == request.FestivalId)
                    .ToListAsync(cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        foreach (var room in rooms)
        {
            var messageCount = 0;
            switch (request.RequestUserType)
            {
                case ChatRequestUserType.User:
                    messageCount = await _unitOfWork.Repository<ChatMessage>()
                        .Entities
                        .Where(p => !(p.Type == ChatMessageType.UserToAdmin || p.Type == ChatMessageType.UserToFestival)
                                    && !p.Seen
                                    && p.ChatRoomId == room.Id)
                        .CountAsync(cancellationToken);
                    break;
                case ChatRequestUserType.Admin:
                    messageCount = await _unitOfWork.Repository<ChatMessage>()
                        .Entities
                        .Where(p =>
                            !(p.Type == ChatMessageType.AdminToFestival || p.Type == ChatMessageType.AdminToUser)
                            && !p.Seen
                            && p.ChatRoomId == room.Id)
                        .CountAsync(cancellationToken);
                    break;
                case ChatRequestUserType.Festival:
                    messageCount = await _unitOfWork.Repository<ChatMessage>()
                        .Entities
                        .Where(p => !(p.Type == ChatMessageType.FestivalToAdmin ||
                                      p.Type == ChatMessageType.FestivalToUser)
                                    && !p.Seen
                                    && p.ChatRoomId == room.Id)
                        .CountAsync(cancellationToken);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var title = string.Empty;

            switch (request.RequestUserType)
            {
                case ChatRequestUserType.User:
                    title = room.Type switch
                    {
                        ChatRoomType.FestivalUser => await _unitOfWork.Repository<Festival>()
                            .Entities.Where(p => p.Id == room.FestivalId)
                            .Select(p => p.Name)
                            .FirstOrDefaultAsync(cancellationToken),
                        ChatRoomType.AdminUser => "Site Admin",
                        _ => title
                    };

                    break;
                case ChatRequestUserType.Admin:
                    title = room.Type switch
                    {
                        ChatRoomType.AdminFestival => await _unitOfWork.Repository<Festival>()
                            .Entities.Where(p => p.Id == room.FestivalId)
                            .Select(p => p.Name)
                            .FirstOrDefaultAsync(cancellationToken),
                        ChatRoomType.AdminUser => _userService.GetAsync(room.ChatUser1).Result.Data.FullName,
                        _ => title
                    };

                    break;
                case ChatRequestUserType.Festival:
                    title = room.Type switch
                    {
                        ChatRoomType.AdminFestival => "Site Admin",
                        ChatRoomType.FestivalUser => _userService.GetAsync(room.ChatUser1).Result.Data.FullName,
                        _ => title
                    };

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            responses.Add(new GetAllRoomResponse
            {
                Title = title,
                Type = room.Type,
                RoomId = room.Id,
                UserId = room.ChatUser1,
                FestivalId = room.FestivalId,
                NotSeenMessageCount = messageCount
            });
        }

        return await Result<List<GetAllRoomResponse>>.SuccessAsync(responses);
    }
}

public class GetAllRoomResponse
{
    public int RoomId { get; set; }
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    
    public  ChatRoomType Type { get; set; }
    public string Title { get; set; }
    public int NotSeenMessageCount { get; set; }
}

