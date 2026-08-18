using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Payments;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;
using HiSubmit.Application.Specifications.Base;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;

using System.Collections.Generic;

namespace HiSubmit.Application.Features.Payments.Queries;

public class GetAllCartItemQuery :PaymentFilterDto,
     IRequest<PaginatedResult<GetCartItemResponse>>
{
    public GetCartItemQueryType Type { get; set; }
}

public enum GetCartItemQueryType
{
    User,
    Festival,
    Admin
}

internal class GetAllCartItemQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork) :
    IRequestHandler<GetAllCartItemQuery, PaginatedResult<GetCartItemResponse>>
{
    public async Task<PaginatedResult<GetCartItemResponse>> Handle
        (GetAllCartItemQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<CarTItem>()
                .Entities
                .Include(p => p.Submit).ThenInclude(p => p.Festival)
                .Include(p => p.Submit)
                .ThenInclude(p => p.SubmitDeadlineEventCategories).ThenInclude(p => p.DeadlineEventCategory)
                .ThenInclude(p => p.EventCategory)
                .Include(p => p.ProductSold).ThenInclude(p => p.Product).ThenInclude(p => p.Festival)
                .Include(p => p.SoldTicket).ThenInclude(p => p.Ticket).ThenInclude(p => p.Venue)
                .ThenInclude(p => p.Festival)
            ;
        switch (request.Type)
        {
            case GetCartItemQueryType.Festival:
                if (request.FestivalId == null || request.FestivalId <= 0)
                    return PaginatedResult<GetCartItemResponse>.Failure(
                        new List<string> { "Festival is required" });
                {
                    var specify = new GetAllFestivalCartItemsSpecification(request.FestivalId,request.MasterFestivalId);
                    var specify2 = new CartItemFilterSpecification(request);
                    var andSpecification = new AndSpecification<CarTItem>(specify, specify2);
                    var festivalCartItems = await query
                        .Specify(andSpecification)
                        .Where(p => string.IsNullOrWhiteSpace(request.SearchString) ||
                                    p.Title.Contains(request.SearchString))
                        .ProjectTo<GetCartItemResponse>(mapper.ConfigurationProvider)
                        .ToPaginatedListAsync(request);
                    return festivalCartItems;
                }

            case GetCartItemQueryType.Admin:
                var specifyAdmin = new GetAllAdminCArtItemFilterSpecification(request);
                var filterSpecify = new CartItemFilterSpecification(request);
                var andSpecificationAdmin=new AndSpecification<CarTItem>(specifyAdmin, filterSpecify);
                var adminCartItems = await query
                        .Specify(andSpecificationAdmin)
                        .Where(p => string.IsNullOrWhiteSpace(request.SearchString) ||
                                    p.Title.Contains(request.SearchString))
                        .ProjectTo<GetCartItemResponse>(mapper.ConfigurationProvider)
                        .ToPaginatedListAsync(request);
                return adminCartItems;

            default:
                return PaginatedResult<GetCartItemResponse>.Failure(
                    new List<string> { "Cart item query type is not supported" });
        }
    }
}
