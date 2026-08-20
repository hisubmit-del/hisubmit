using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Events.TicketsSold;
using HiSubmit.Application.Interfaces.GenerateQrCode;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.SoldTickets.Commands;

public record AddSoldTicketCommand : IRequest<IResult>
{
    public int Count { get; set; }
    public  int VenueId { get; set; }
    public int TicketId { get; set; }
    public int? ShowTimeId { get; set; }
    public int? ChairNumber { get; set; }
    public  bool ForOtherUser { get; set; }
    public  string OtherUserEmail { get; set; }
}

public class AddSoldTicketCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<AddSoldTicketCommandHandler> localizer,
    IMapper mapper,
    ICurrentUserService currentUserService,
    IMediator mediator,
    IGenerateQrCode generateQrCode)
    : IRequestHandler<AddSoldTicketCommand, IResult>
{
    public async Task<IResult> Handle(AddSoldTicketCommand request, CancellationToken cancellationToken)
    {
        if (request.TicketId <= 0 || request.Count <= 0)
            return await Result.FailAsync(localizer["A valid ticket and quantity are required"]);

        if (currentUserService == null || string.IsNullOrWhiteSpace(currentUserService.UserId))
            return await Result.FailAsync(localizer["You must be signed in to add a ticket to your cart"]);

        var ticket = await unitOfWork.Repository<Ticket>().GetByIdAsync(request.TicketId);
        if (ticket == null)
            return await Result.FailAsync(localizer["Ticket not found"]);

        if (request.ShowTimeId != null)
        {
            var showTime = await unitOfWork.Repository<ShowTime>().GetByIdAsync(request.ShowTimeId.Value);
            if (showTime == null)
                return await Result.FailAsync(localizer["Show time not found"]);
            if (showTime.AvailableCapacity < request.Count)
            {
                return await Result.FailAsync(localizer["The capacity of the hall for this session is full"]);
            }

            if (ticket.AvailableCapacity < request.Count)
            {
                return await Result.FailAsync(localizer["The ticket capacity is full"]);
            }
        }

        var commissionRepository = unitOfWork.Repository<SiteCommission>();
        if (commissionRepository == null)
            return await Result.FailAsync(localizer["Site commission settings are unavailable"]);

        var commission = await commissionRepository.Entities
            .FirstOrDefaultAsync(cancellationToken);
        if (commission == null)
            return await Result.FailAsync(localizer["Site commission settings are not configured"]);
        
        var soldTicket = mapper.Map<SoldTicket>(request);
        if (soldTicket == null)
            return await Result.FailAsync(localizer["The ticket could not be added to your shopping cart"]);
        
        soldTicket.SoldTicketStatus = SoldTicketStatus.AwaitingPayment;
        soldTicket.UserId = currentUserService.UserId;
        soldTicket.Cost = ticket.Cost * request.Count;
        soldTicket.ShareFestivalIncome =
            (decimal)(100 - commission.TicketSalesCommission) / 100m * soldTicket.Cost;
        soldTicket.SerialNumber = Guid.NewGuid();
        soldTicket.QrCode =await generateQrCode.Generate(soldTicket.SerialNumber.ToString());
        
        await unitOfWork.Repository<SoldTicket>().AddAsync(soldTicket);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new TicketSoldEvent() { TicketSoldId = soldTicket.Id},cancellationToken);
        return await Result.SuccessAsync(localizer["The ticket has been added to your shopping cart"]);
    }
}
