using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CartItemType = HiSubmit.Domain.Enums.CartItemType;

namespace HiSubmit.Application.Features.Payments.DiscountsCodes.Queries;

public class CalculateDiscountCodeQuery : CalculateDiscountCodesRequest, IRequest<PaginatedResult<GetCartItemResponse>>;

public class CalculateDiscountCodeQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    : IRequestHandler<CalculateDiscountCodeQuery, PaginatedResult<GetCartItemResponse>>
{
    private List<DiscountCode> _validDiscountCodes = [];
    public async Task<PaginatedResult<GetCartItemResponse>> Handle(CalculateDiscountCodeQuery request, CancellationToken cancellationToken)
    {
        var cart = await unitOfWork.Repository<Cart>()
            .Entities
            .Include(p => p.CartItems).ThenInclude(p=>p.ProductSold).ThenInclude(p=>p.Product).ThenInclude(p=>p.Festival)
            .Include(p => p.CartItems).ThenInclude(p=>p.SoldTicket).ThenInclude(p=>p.Ticket).ThenInclude(p=>p.Venue).ThenInclude(p=>p.Festival)
            .Include(p => p.CartItems).ThenInclude(p=>p.Submit).ThenInclude(p => p.Festival)
            .Where(p => p.Id == request.CartId)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart == null || cart.Paid)
            return new PaginatedResult<GetCartItemResponse>([])
            {
                Succeeded = false,
                Messages = ["Your cart not found or paid"]
            };

        if (!request.DiscountCodes.Any())
            return new PaginatedResult<GetCartItemResponse>(mapper.Map<List<GetCartItemResponse>>(cart.CartItems))
            {
                Succeeded = true,
            };

        
        await GetEnableDiscountCode(request.DiscountCodes);

        foreach (var cc in cart.CartItems.Where(p=>p.CartItemType!=CartItemType.ServiceFee))
        {
            var minPrice = cc.Price;
            int? selectedDiscountCode = cc.Id;
            var codes = GetValidCodeForItem(cc);
            foreach (var c in codes)
            {
                var discountPrice = GetDiscountPrice(c, cc);
                if (discountPrice < minPrice)
                {
                    minPrice = discountPrice;
                    selectedDiscountCode = c.Id;
                }
            }

            if (minPrice < 0)
                minPrice = 0;
            if (minPrice < cc.Price)
            {
                cc.PriceAfterDiscount = minPrice;
                cc.DiscountCodeId=selectedDiscountCode;
            }
        }

        return new PaginatedResult<GetCartItemResponse>(mapper.Map<List<GetCartItemResponse>>(cart.CartItems))
        {
            Succeeded = true,
            TotalCount = cart.CartItems.Count
        };
    }

    private async Task GetEnableDiscountCode(List<string> codes)
    {
        var discountCodes = await unitOfWork.Repository<DiscountCode>()
            .Entities
            .Where(p => codes.Any(code => code == p.Code))
            .ToListAsync();

        //check Enables
        var validCodes = discountCodes.Where(p => p.Enable).ToList();

        //;Check Expired date
         validCodes = validCodes.Where(p => p.ExpiredTime == null ||
                                                  p.ExpiredTime.Value.Date >= DateTime.Today).ToList();
        //check Count
        foreach (var vc in validCodes.Where(p=>p.Count!=null).ToList())
        {
            var countOfUsed = await unitOfWork.Repository<CarTItem>()
                .Entities
                .Where(p => p.DiscountCodeId == vc.Id)
                .CountAsync();
            if (countOfUsed >= vc.Count)
                validCodes.Remove(vc);
        }

        _validDiscountCodes= validCodes;
    }

    private List<DiscountCode>  GetValidCodeForItem(CarTItem item)
    {
        switch (item.CartItemType)
        {
            case CartItemType.Submit:
                return _validDiscountCodes.Where(p => p.FestivalId == item.Submit.FestivalId
                                                     && HasType(p, CartItemType.Submit)).ToList();
            case CartItemType.Badge or  CartItemType.Ticket:
                return _validDiscountCodes.Where(p => p.FestivalId == item.SoldTicket.Ticket.Venue.FestivalId
                                                     && HasType(p, CartItemType.Ticket)).ToList();
            case CartItemType.Product:
                return _validDiscountCodes.Where(p => p.FestivalId == item.ProductSold.Product.FestivalId
                                                     && HasType(p, CartItemType.Product)).ToList();
        }
        return [];
    }

    private decimal GetDiscountPrice(DiscountCode dc, CarTItem ct)
    {
        if (dc.DiscountValueType == DiscountValueType.Amount)
            return ct.Price -(decimal) dc.DiscountValue;
        return (ct.Price)- (ct.Price * (decimal) dc.DiscountValue /100);
    }
    private bool HasType(DiscountCode dc, CartItemType type)
    {
        return dc.CartItemTypes.Contains(((byte)type).ToString());
    }
}
