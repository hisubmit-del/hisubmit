using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;
using Hisubmit.Hisubmit.Client.SharedModels.Features.AdminDashboard;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.AdminDashboard.Queries;

public class GetSitePurchaseQuery : GetSitePurchaseRequest, IRequest<IResult<GetSitePurchaseResponse>>;

public class GetSitePurchaseQueryHandler(IUnitOfWork<int> unitOfWork)
    : IRequestHandler<GetSitePurchaseQuery, IResult<GetSitePurchaseResponse>>
{
    private GetSitePurchaseResponse _response = new();
    public async Task<IResult<GetSitePurchaseResponse>> Handle(GetSitePurchaseQuery request, CancellationToken cancellationToken)
    {
        var specify = new SitePurchaseFilterSpecification(request);

        var query = unitOfWork.Repository<CarTItem>()
            .Entities
            .Specify(specify);


        await CalculateSubmissionFee(query);

        await CalculateServiceFee(query);

        var productSoldIds = await query
            .Where(p => p.CartItemType == CartItemType.Product)
            .Select(p => p.ProductSoldId).ToArrayAsync(cancellationToken);
        await CalculateProductIncomes(productSoldIds, cancellationToken);

        var ticketSoldIds = await query
            .Where(p => p.CartItemType == CartItemType.Ticket || p.CartItemType== CartItemType.Badge)
            .Select(p => p.SoldTicketId).ToArrayAsync(cancellationToken);

        await CalculateTicketIncomes(ticketSoldIds, cancellationToken);

        return await Result<GetSitePurchaseResponse>.SuccessAsync(_response);
    }


   
    private async Task CalculateSubmissionFee(IQueryable<CarTItem> query)
    {
        var submission = await query
                .Where(p => p.CartItemType == CartItemType.Submit && p.PriceAfterDiscount!=null)
                .SumAsync(p => p.PriceAfterDiscount)
            ;

        submission += await query
                .Where(p => p.CartItemType == CartItemType.Submit && p.PriceAfterDiscount==null)
                .SumAsync(p => p.Price)
            ;

        _response.ServiceFee = submission??0;
    }


    private async Task CalculateServiceFee(IQueryable<CarTItem> query)
    {
        var serviceFee = await query
                .Where(p => p.CartItemType == CartItemType.ServiceFee && p.PriceAfterDiscount!=null)
                .SumAsync(p => p.PriceAfterDiscount)
            ;
        serviceFee += await query
                .Where(p => p.CartItemType == CartItemType.ServiceFee && p.PriceAfterDiscount==null)
                .SumAsync(p => p.Price)
            ;
        _response.ServiceFee = serviceFee ??0;
    }

    public async Task CalculateProductIncomes(int?[] productSoldIds, CancellationToken cancellationToken)
    {
        var productSold = await unitOfWork.Repository<ProductSold>()
            .Entities
            .Where(p => productSoldIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        _response.AllProduct = productSold.Sum(p => p.Income);
        _response.SiteProduct = productSold.Sum(p => p.Income - p.ShareFestivalIncome);
    }

    public async Task CalculateTicketIncomes(int?[] ticketSoldIds, CancellationToken cancellationToken)
    {
        var productSold = await unitOfWork.Repository<SoldTicket>()
            .Entities
            .Where(p => ticketSoldIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        _response.AllTicket = productSold.Sum(p => p.Cost);
        _response.SiteTicket = productSold.Sum(p => p.Cost - p.ShareFestivalIncome);
    }
}