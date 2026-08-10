using AutoMapper;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Specifications.Payments;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Specifications.Base;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using Hisubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Application.Features.Payments.Queries;

public class GetAllCartsQuery : GetAllCartsFilterDto, IRequest<PaginatedResult<GetAllCartsResponse>>
{
    public GetCartItemQueryType Type { get; set; }
}

public class GetAllCartsQueryHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    IUserService userService,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetAllCartsQuery, PaginatedResult<GetAllCartsResponse>>
{
    public async Task<PaginatedResult<GetAllCartsResponse>> Handle(GetAllCartsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.TakeCurrentUserCarts)
            request.UserId = request.TakeCurrentUserCarts ? currentUserService.UserId : request.UserId;


        var filterSpecification = new GetAllCartsFilterSpecification(request);
        var userSpecification = new GetUserCartsSpecification(request.UserId);
        var quickSearchSpecification = new AllCartQuickSearchSpecification(request.SearchString);

        var andSpecifiy = new AndSpecification<Cart>
            (filterSpecification, userSpecification, quickSearchSpecification);

        var carts = await unitOfWork.Repository<Cart>()
            .Entities
            .Specify(andSpecifiy)
            .Include(p => p.CartItems)
            .Include(p => p.CartItems).ThenInclude(p => p.Submit).ThenInclude(p => p.Festival)
            .Include(p => p.CartItems).ThenInclude(p => p.Submit)
            .ThenInclude(p => p.SubmitDeadlineEventCategories).ThenInclude(p => p.DeadlineEventCategory)
            .ThenInclude(p => p.EventCategory)
            .Include(p => p.CartItems).ThenInclude(p => p.ProductSold).ThenInclude(p => p.Product)
            .ThenInclude(p => p.Festival)
            .Include(p => p.CartItems).ThenInclude(p => p.SoldTicket).ThenInclude(p => p.Ticket)
            .ThenInclude(p => p.Venue).ThenInclude(p => p.Festival)
            .ProjectTo<GetAllCartsResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        var items = new List<GetAllCartsResponse>();
        if (carts != null) items.AddRange(carts.Data.Select(c => mapper.Map<GetAllCartsResponse>(c)));

        var users = await userService
            .GetAllAsync(items.Select(p => p.UserId).ToList());

        foreach (var u in users.Data)
        foreach (var i in items.Where(p => p.UserId == u.Id))
            i.UserFullName = u.FullName;

        return new PaginatedResult<GetAllCartsResponse>(items)
        {
            PageSize = carts.PageSize,
            CurrentPage = carts.CurrentPage,
            Succeeded = carts.Succeeded,
            Messages = carts.Messages,
            TotalCount = carts.TotalCount,
            TotalPages = carts.TotalPages
        };
    }
}