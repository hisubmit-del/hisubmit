using System;
using MediatR;
using System.Linq;
using System.Threading;
using HiSubmit.Domain.Enums;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.EntityFrameworkCore;
using HiSubmit.Domain.Entities.Payments;
using Microsoft.Extensions.Localization;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Events.Payments.DeleteFestivalSubmitItemInCartItem;

namespace HiSubmit.Application.Features.Payments.Commands;

public class DeleteCartItemCommand : IRequest<IResult>
{
    public int Id { get; set; }
}

public class DeleteCartItemCommandHandler : IRequestHandler<DeleteCartItemCommand, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<DeleteCartItemCommandHandler> _localize;
    private readonly IMediator _mediator;

    public DeleteCartItemCommandHandler
    (IUnitOfWork<int> unitOfWork, ICurrentUserService currentUserService,
        IStringLocalizer<DeleteCartItemCommandHandler> localize,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _localize = localize;
        _mediator = mediator;
    }

    public async Task<IResult> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<CarTItem>()
            .Entities
            .Include(p => p.Cart)
            .Where(p => p.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (item == null || item.Cart.Paid || item.Cart.UserId != _currentUserService.UserId)
            throw new NullReferenceException("Item not found");

        if (item.CartItemType == CartItemType.Submit)
        {
            await _mediator.Publish(new DeletedFestivalSubmitItemInCartItemEvent
                {
                    SubmitId = item.SubmitId.Value
                },cancellationToken);
                
                var serviceFeeItem = await _unitOfWork.Repository<CarTItem>()
                    .Entities.Where(p => p.ItemId == item.ItemId && p.CartItemType == CartItemType.ServiceFee)
                    .FirstOrDefaultAsync(cancellationToken);
            
                if (serviceFeeItem != null)
                {
                    await _unitOfWork.Repository<CarTItem>()
                        .DeleteAsync(serviceFeeItem);
                }
        }

        await _unitOfWork.Repository<CarTItem>().DeleteAsync(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}

