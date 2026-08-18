using System;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommand:IRequest<IResult>
{
    public  int FestivalId { get; set; }
    public  int Id { get; set; }
}

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<DeleteTicketCommandHandler> _localizer;

    public DeleteTicketCommandHandler
        (IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteTicketCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }

    public async Task<IResult> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Repository<Ticket>()
            .GetByIdAsync(request.Id);

        if (ticket == null)
            return await Result.FailAsync(_localizer["The ticket was not found"]);

        if (!ticket.VenueId.HasValue)
            return await Result.FailAsync(_localizer["The ticket has no festival assigned"]);

        var venue = await _unitOfWork.Repository<HiSubmit.Domain.Entities.Festivals.Venue>()
            .GetByIdAsync(ticket.VenueId.Value);
        if (venue == null || venue.FestivalId != request.FestivalId)
            return await Result.FailAsync(_localizer["The ticket does not belong to this festival"]);

        if (ticket.OpenDate < DateTime.Now)
        {
            return await Result.FailAsync(_localizer["The time for ticket sales has started"]);
        }

        await _unitOfWork.Repository<Ticket>()
            .DeleteAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(_localizer["The ticket was successfully deleted"]);
    }
}
