using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Entities.Chats;
using HiSubmit.Domain.Enums.Chats;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Chats.Queries;

public class GetChatHistoryQuery : IRequest<IResult<List<GetChatHistoryResponse>>>
{
    public int RoomId { get; set; }
    public ChatRequestUserType Type { get; set; }
}

public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, IResult<List<GetChatHistoryResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetChatHistoryQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper
    ,ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<IResult<List<GetChatHistoryResponse>>> Handle(GetChatHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var messages = await _unitOfWork.Repository<ChatMessage>()
            .Entities
            .Where(p => p.ChatRoomId == request.RoomId)
            .ToListAsync(cancellationToken);

        switch (request.Type)
        {
            case ChatRequestUserType.User:
                foreach (var message in messages.Where(p =>
                             p.Type is not (ChatMessageType.UserToAdmin or ChatMessageType.UserToFestival)))
                {
                    message.Seen = true;
                    await _unitOfWork.Repository<ChatMessage>().UpdateAsync(message);
                }

                break;
            case ChatRequestUserType.Admin:
                foreach (var message in messages.Where(p =>
                             p.Type is not (ChatMessageType.AdminToFestival or ChatMessageType.AdminToUser)))
                {
                    message.Seen = true;
                    await _unitOfWork.Repository<ChatMessage>().UpdateAsync(message);
                }

                break;
            case ChatRequestUserType.Festival:
                foreach (var message in messages.Where(p =>
                             p.Type is not (ChatMessageType.FestivalToAdmin or ChatMessageType.FestivalToUser)))
                {
                    message.Seen = true;
                    await _unitOfWork.Repository<ChatMessage>().UpdateAsync(message);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result<List<GetChatHistoryResponse>>.SuccessAsync(
            _mapper.Map<List<GetChatHistoryResponse>>(messages));
    }
}

public class GetChatHistoryResponse
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string UserId { get; set; }
    public ChatMessageType Type { get; set; }
    public int ChatRoomId { get; set; }
    public DateTime CreatedOn { get; set; }
}