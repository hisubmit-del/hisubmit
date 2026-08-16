using System;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Tickets.Commands.Enable;

public class EnableTicketCommand : Hisubmit.Client.SharedModels.Features.Tickets.Commands.Enable.EnableTicketCommand,
    IRequest<IResult>;

public class EnableTicketCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<EnableTicketCommandHandler> localizer)
    : IRequestHandler<EnableTicketCommand, IResult>
{
    public async Task<IResult> Handle(EnableTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await unitOfWork.Repository<Ticket>()
            .GetByIdAsync(request.TicketId);

        if (ticket == null)
            throw new NullReferenceException();

       // ticket.Status = request.Status;
        ticket.IsEnable=request.IsEnable;
        await unitOfWork.Repository<Ticket>().UpdateAsync(ticket);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(localizer["Ticket Updated"]);
    }
}
