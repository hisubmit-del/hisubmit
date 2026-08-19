using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.Payments.PaidCartEvent;
using HiSubmit.Application.Exceptions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Services.PaymentService;
using Hisubmit.Client.SharedModels.Features.Payments.Commands;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Payments.Commands;

public class PaidZeroCartCommand : PaidZeroCartRequest, IRequest<IResult>
{
    public string UserId { get; set; }
}

public class PaidZeroCartCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPaymentService paymentService)
    : IRequestHandler<PaidZeroCartCommand, IResult>
{
    public async Task<IResult> Handle(PaidZeroCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await unitOfWork.Repository<Cart>()
            .Entities.Where(p => p.UserId == request.UserId && !p.Paid)
            .Include(p => p.CartItems)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
            return await Result.FailAsync("Your open cart was not found.");

        if (cart.CartItems.Sum(p => p.Price) != 0)
        {
            throw new BadRequestException();
        }

        var res = await paymentService.ChangeCartItemState(cart, cancellationToken);

        if (res.Succeeded)
        {
            cart.CartDate = DateTime.Now;
            cart.Paid = true;

            await unitOfWork.Repository<Cart>().UpdateAsync(cart);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new CartPaidedEvent() { CartId = cart.Id }, cancellationToken);
            return await Result<int>.SuccessAsync(cart.Id);
        }
        return res;
    }
}
