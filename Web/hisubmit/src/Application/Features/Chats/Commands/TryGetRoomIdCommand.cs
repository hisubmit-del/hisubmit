using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Chats;
using HiSubmit.Domain.Enums.Chats;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Chats.Commands;

public class TryGetRoomIdCommand : IRequest<IResult<int>>
{
    public string ChatUser1 { get; set; }
    public string ChatUser2 { get; set; }
    public bool ChatWithAdmin { get; set; }
    public int? FestivalId { get; set; }
    public ChatRoomType Type { get; set; }
}

public class TryGetRoomIdCommandHandler : IRequestHandler<TryGetRoomIdCommand, IResult<int>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<TryGetRoomIdCommandHandler> _localize;

    public TryGetRoomIdCommandHandler
    (
        IMapper mapper,
        IUnitOfWork<int> unitOfWork,
        IStringLocalizer<TryGetRoomIdCommandHandler> localize)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localize = localize;
    }

    public async Task<IResult<int>> Handle(TryGetRoomIdCommand request, CancellationToken cancellationToken)
    {
        var chatRoom = await _unitOfWork.Repository<ChatRoom>()
            .Entities
            .Where(p =>
                p.Type == request.Type &&
                p.ChatUser1 == request.ChatUser1
                && p.ChatUser2 == request.ChatUser2
                && p.FestivalId == request.FestivalId
                && p.ChatWithAdmin == request.ChatWithAdmin)
            .FirstOrDefaultAsync(cancellationToken);

        if (chatRoom != null) return await Result<int>.SuccessAsync(chatRoom.Id);

        chatRoom = _mapper.Map<ChatRoom>(request);
        await _unitOfWork.Repository<ChatRoom>().AddAsync(chatRoom);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await Result<int>.SuccessAsync(chatRoom.Id);
    }
}