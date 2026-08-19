using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.FestivalPaymentItems.Queries.FestivalPaymentStates;

public class GetFestivalPaymentStateQuery:IRequest<IResult<GetFestivalPaymentStateResponse>>
{
    public  int FestivalId { get; set; }
}

public  class GetFestivalPaymentStateQueryHandler:IRequestHandler<GetFestivalPaymentStateQuery,IResult<GetFestivalPaymentStateResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetFestivalPaymentStateQueryHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<GetFestivalPaymentStateResponse>> Handle(GetFestivalPaymentStateQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<ProductSold>()
            .Entities
            .Include(p=>p.Product)
            .Where(p => p.Product.FestivalId == request.FestivalId &&
                        p.Status == ProductSoldStatus.Paid)
            .SumAsync(p => p.ShareFestivalIncome,cancellationToken);

        var ticket = await _unitOfWork.Repository<SoldTicket>()
            .Entities
            .Include(p=>p.Ticket).ThenInclude(p=>p.Venue)
            .Where(p => p.Ticket.Venue.FestivalId == request.FestivalId &&
                        p.SoldTicketStatus == SoldTicketStatus.Paid)
            .SumAsync(p => p.ShareFestivalIncome, cancellationToken);

        var submit = await _unitOfWork.Repository<CarTItem>()
            .Entities
            .Where(p => p.Cart.Paid &&
                        p.CartItemType == CartItemType.Submit
                        && p.Submit.FestivalId == request.FestivalId)
            .SumAsync(p => p.PriceAfterDiscount ?? p.Price, cancellationToken);

        var siteCharges = await _unitOfWork.Repository<CarTItem>()
            .Entities
            .Where(p => p.Cart.Paid &&
                        p.CartItemType == CartItemType.ServiceFee &&
                        p.Submit.FestivalId == request.FestivalId)
            .SumAsync(p => p.PriceAfterDiscount ?? p.Price, cancellationToken);

        var lastMonth = DateTime.Now.AddMonths(-1);
        var lastMonthProduct = await _unitOfWork.Repository<CarTItem>()
            .Entities
            .Where(p => p.Cart.Paid &&
                        p.Cart.CartDate >= lastMonth &&
                        p.CartItemType == CartItemType.Product &&
                        p.ProductSold.Status == ProductSoldStatus.Paid &&
                        p.ProductSold.Product.FestivalId == request.FestivalId)
            .SumAsync(p => p.ProductSold.ShareFestivalIncome, cancellationToken);

        var lastMonthTicket = await _unitOfWork.Repository<CarTItem>()
            .Entities
            .Where(p => p.Cart.Paid &&
                        p.Cart.CartDate >= lastMonth &&
                        (p.CartItemType == CartItemType.Ticket ||
                         p.CartItemType == CartItemType.Badge) &&
                        p.SoldTicket.SoldTicketStatus == SoldTicketStatus.Paid &&
                        p.SoldTicket.Ticket.Venue.FestivalId == request.FestivalId)
            .SumAsync(p => p.SoldTicket.ShareFestivalIncome, cancellationToken);

        var lastMonthSubmit = await _unitOfWork.Repository<CarTItem>()
            .Entities
            .Where(p => p.Cart.Paid &&
                        p.Cart.CartDate >= lastMonth &&
                        p.CartItemType == CartItemType.Submit &&
                        p.Submit.FestivalId == request.FestivalId)
            .SumAsync(p => p.PriceAfterDiscount ?? p.Price, cancellationToken);

        
        var items = await _unitOfWork.Repository<FestivalPaymentItem>()
            .Entities
            .Where(p => p.FestivalId == request.FestivalId)
            .SumAsync(p => p.Amount, cancellationToken);

        var result = new GetFestivalPaymentStateResponse
        {
            FestivalDebt = product + ticket + submit - (decimal)items,
            FestivalId = request.FestivalId,
            Product = product,
            Ticket = ticket,
            Submit = submit,
            AdminPayment = (decimal) items,
            Income = product + ticket + submit,
            LastMonthIncome = lastMonthProduct + lastMonthTicket + lastMonthSubmit,
            SiteCharges = siteCharges,
            NetSettlementDue = product + ticket + submit - (decimal)items - siteCharges
        };

        return await Result<GetFestivalPaymentStateResponse>.SuccessAsync(result);
    }
}

