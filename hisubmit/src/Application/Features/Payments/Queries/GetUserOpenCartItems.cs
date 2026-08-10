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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;

namespace HiSubmit.Application.Features.Payments.Queries;

public class GetUserOpenCartItemQuery : IRequest<Result<List<GetCartItemResponse>>>
{
    public string UserId { get; set; }
}
    
public class GetUserOpenCartItemsQueryHandler : IRequestHandler<GetUserOpenCartItemQuery, Result<List<GetCartItemResponse>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    public GetUserOpenCartItemsQueryHandler(
        IUnitOfWork<int> unitOfWork, ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<List<GetCartItemResponse>>> Handle(GetUserOpenCartItemQuery request, CancellationToken cancellationToken)
    {
        var userId = string.IsNullOrWhiteSpace(request.UserId) ? _currentUserService.UserId : request.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return await Result<List<GetCartItemResponse>>.FailAsync("User not Found");

        var specification = new GetOpenCartUserSpecification(userId);

        var userCart = await _unitOfWork.Repository<Cart>()
            .Entities.Specify(specification)
            .Include(p => p.CartItems).ThenInclude(p=>p.Submit)
            .ThenInclude(p=>p.Festival)
            .Include(p => p.CartItems).ThenInclude(p=>p.Submit)
            .ThenInclude(p=>p.SubmitDeadlineEventCategories)
            .ThenInclude(p=>p.DeadlineEventCategory)
            .ThenInclude(p=>p.EventCategory)
            .Include(p=>p.CartItems)
            .ThenInclude(p=>p.ProductSold).ThenInclude(p=>p.Product).ThenInclude(p=>p.Festival)
            .Include(p=>p.CartItems)
            .ThenInclude(p=>p.SoldTicket).ThenInclude(p=>p.Ticket)
            .ThenInclude(p=>p.Venue).ThenInclude(p=>p.Festival)
            .FirstOrDefaultAsync(cancellationToken);

        var items = new List<GetCartItemResponse>();
        if(userCart is { CartItems: { } })
        {
            items = _mapper.Map<List<GetCartItemResponse>>(userCart.CartItems);
        }
        if (userCart != null)
        {
            return await Result<List<GetCartItemResponse>>.SuccessAsync(items);
        }
        return await Result<List<GetCartItemResponse>>.SuccessAsync(items);
    }
}