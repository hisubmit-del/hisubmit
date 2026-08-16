using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Events.Chats.MessageSended;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Features.Chats.Commands;
using HiSubmit.Domain.Entities.Chats;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;

namespace HiSubmit.Application.Features.Chats.Commands;

public class AddChatMessageCommand : AddChatMessageRequest, IRequest<IResult>;


public class AddChatMessageCommandHandler(IMapper mapper, IUnitOfWork<int> unitOfWork,IMediator mediator)
    : IRequestHandler<AddChatMessageCommand, IResult>
{
    public async Task<IResult> Handle(AddChatMessageCommand request, CancellationToken cancellationToken)
    {
        var message = mapper.Map<ChatMessage>(request);
        var chatRoom = await unitOfWork.Repository<ChatRoom>()
            .GetByIdAsync(request.ChatRoomId);
        chatRoom.LastModifiedTime = DateTime.Now;
        await unitOfWork.Repository<ChatMessage>().AddAsync(message);
        await unitOfWork.Repository<ChatRoom>().UpdateAsync(chatRoom); 
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish
            (new MessageSendedEvent { ChatMessageType = request.Type, ChatRoom = chatRoom },
                cancellationToken);
        return await Result.SuccessAsync();
    }
}
