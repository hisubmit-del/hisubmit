using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Events.TicketsSold;
using HiSubmit.Application.Interfaces.GenerateQrCode;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.SoldTickets.Commands;

public class AddSoldBadgeCommand:IRequest<IResult>
{
    public  string OtherUserEmail { get; set; }
    public  bool ForOtherUser { get; set; }
    public  int TicketId { get; set; }
    public  int Count { get; set; }
}

public class AddSoldCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<AddSoldCommandHandler> localizer,
    IMapper mapper,
    IGenerateQrCode generateQrCode,
    ICurrentUserService currentUserService,
    IMediator mediator)
    : IRequestHandler<AddSoldBadgeCommand, IResult>
{
    public async Task<IResult> Handle(AddSoldBadgeCommand request, CancellationToken cancellationToken)
    {
        var ticket = await unitOfWork.Repository<Ticket>().GetByIdAsync(request.TicketId);
        if (ticket == null)
            return await Result.FailAsync(localizer["Ticket not found"]);
        if (ticket.AvailableCapacity  < request.Count)
        {
            return await Result.FailAsync(localizer["Ticket sales capacity has expired"]);
        }

        var commission = await unitOfWork.Repository<SiteCommission>()
            .Entities.FirstOrDefaultAsync(cancellationToken);
        
        var cost =(int)(ticket.AddManagerPercentage ? ticket.Cost * 1.2 : ticket.Cost);
        var soldTicket = mapper.Map<SoldTicket>(request);
        soldTicket.SoldTicketStatus = SoldTicketStatus.AwaitingPayment;
        soldTicket.Cost = cost;
        soldTicket.ShareFestivalIncome =(decimal) (100 - commission.TicketSalesCommission) 
                                        * soldTicket.Cost;
        soldTicket.UserId = currentUserService.UserId;
        soldTicket.SerialNumber = Guid.NewGuid();
        
        soldTicket.QrCode =await generateQrCode.Generate(soldTicket.SerialNumber.ToString());
        await  unitOfWork.Repository<SoldTicket>().AddAsync(soldTicket);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new BadgeSoldEvent(){TicketSoldId = soldTicket.Id},cancellationToken);
        return await Result.SuccessAsync(localizer["badge added to your cart"]);
    }
}