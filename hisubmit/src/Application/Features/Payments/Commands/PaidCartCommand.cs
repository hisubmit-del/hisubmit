using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Specifications.Payments;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.Payments.PaidCartEvent;
using HiSubmit.Application.Events.Submits.PaidSubmit;
using HiSubmit.Application.Events.TicketsSold;
using HiSubmit.Application.Features.Payments.DiscountsCodes.Queries;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Services.PaymentService;
using Hisubmit.Client.SharedModels.Features.Payments.Commands;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.Payments.Commands;

public class PaidCartCommand : PaidCartRequest, IRequest<Result<CheckPaymentResponse>>;

public class PaidCartCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IMediator mediator,
    ICurrentUserService currentUserService,
    IUserService userService,
    IPayPalService payPalService)
    : IRequestHandler<PaidCartCommand, Result<CheckPaymentResponse>>
{
    public async Task<Result<CheckPaymentResponse>> Handle(PaidCartCommand request, CancellationToken cancellationToken)
    {

        var cartItemResponse = await mediator.Send(new CalculateDiscountCodeQuery()
        {
            DiscountCodes = request.DiscountCodes,
            CartId = request.CartId
        });

        if (!cartItemResponse.Succeeded || cartItemResponse.Data is null)
            return await Result<CheckPaymentResponse>.FailAsync(
                cartItemResponse.Messages?.ToList() ?? ["Unable to calculate the cart total."]);

        var realPrice = cartItemResponse.Data.Sum(p => p.GetRealPrice());

        var cart = await unitOfWork.Repository<Cart>()
            .Entities
            .Include(p => p.CartItems)
            .Where(p => p.Id == request.CartId &&
                        p.UserId == currentUserService.UserId &&
                        !p.Paid)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
            return await Result<CheckPaymentResponse>.FailAsync("Your open cart was not found.");

        if (string.IsNullOrWhiteSpace(request.OrderId))
            return await Result<CheckPaymentResponse>.FailAsync("PayPal order reference is required.");

        var duplicatePayment = await unitOfWork.Repository<Cart>()
            .Entities
            .AnyAsync(p => p.Paid &&
                           (p.OrderId == request.OrderId ||
                            (!string.IsNullOrWhiteSpace(request.PaymentId) &&
                             p.PaymentId == request.PaymentId)),
                cancellationToken);

        if (duplicatePayment)
            return await Result<CheckPaymentResponse>.FailAsync("This PayPal payment has already been recorded.");

        try
        {
            var verifiedOrder = await payPalService.VerifyOrderAsync(request.OrderId, realPrice);
            var verifiedPurchaseUnit = verifiedOrder.PurchaseUnits?.FirstOrDefault();
            if (!string.Equals(verifiedOrder.Id, request.OrderId, StringComparison.OrdinalIgnoreCase) ||
                !int.TryParse(verifiedPurchaseUnit?.CustomId, out var verifiedCartId) ||
                verifiedCartId != request.CartId)
                return await Result<CheckPaymentResponse>.FailAsync("PayPal order verification failed.");
        }
        catch (Exception exception) when (exception is HttpRequestException ||
                                          exception is InvalidOperationException ||
                                          exception is JsonException)
        {
            return await Result<CheckPaymentResponse>.FailAsync(
                "PayPal payment verification failed. The cart was not marked as paid.");
        }

        var siteCommission = await unitOfWork.Repository<SiteCommission>()
            .Entities.FirstOrDefaultAsync(cancellationToken);
        
        cart.CartDate=DateTime.Now;
        cart.Email = request.Email;
        cart.OrderId = request.OrderId;
        cart.PayerId = request.PayerId;
        cart.PaymentId = request.PaymentId;
        cart.Price = realPrice;
        cart.Paid = true;

        /////////////////Start Paid Submit Item
        foreach (var submit in cart.CartItems.Where(p => p.CartItemType == CartItemType.Submit))
        {
            await mediator.Publish(new PaidSubmitEvent
                    { SubmitId = submit.SubmitId.Value },
                cancellationToken);
        }
        /////////////////End Paid Submit Item

        /////////////////Start  Badge Item
        var soldBadgesId = cart.CartItems.Where(p => p.CartItemType == CartItemType.Badge)
            .Select(item => int.Parse(item.ItemId)).ToList();
        var notAvailableTicket = false;
        var ticketMessages = new List<string>();


        foreach (var item in soldBadgesId)
        {
            var soldTicket = await unitOfWork.Repository<SoldTicket>()
                .Entities.Include(p => p.Ticket)
                .FirstOrDefaultAsync(p => p.Id == item, cancellationToken);
            if (soldTicket.Ticket.OpenDate < DateTime.Now
                && soldTicket.Ticket.CloseDate > DateTime.Now
                && soldTicket.Count <= soldTicket.Ticket.AvailableCapacity)
            {
                soldTicket.SoldTicketStatus = SoldTicketStatus.Paid;
                soldTicket.Ticket.AvailableCapacity -= soldTicket.Count;
                await unitOfWork.Repository<SoldTicket>().UpdateAsync(soldTicket);
                await mediator.Publish(new PaidBadgeEvent() { SoldTicketId = item }, cancellationToken);
            }
            else
            {
                notAvailableTicket = true;
                ticketMessages.Add($"{soldTicket.Ticket.Title} not available");
            }
        }
        /////////////////Sold Badge Item

        var soldTicketsId = cart.CartItems
            .Where(p => p.CartItemType == CartItemType.Ticket)
            .Select(item => int.Parse(item.ItemId)).ToList();

        foreach (var item in soldTicketsId)
        {
            var soldTicket = await unitOfWork.Repository<SoldTicket>()
                .Entities
                .Include(p => p.Ticket)
                .Include(p => p.ShowTime)
                .FirstOrDefaultAsync(p => p.Id == item, cancellationToken);

            if (soldTicket.Ticket.OpenDate < DateTime.Now && soldTicket.Ticket.CloseDate > DateTime.Now
                                                          && soldTicket.Count <
                                                          soldTicket.Ticket.AvailableCapacity &&
                                                          soldTicket.Count <= soldTicket.ShowTime.AvailableCapacity)
            {
                soldTicket.SoldTicketStatus = SoldTicketStatus.Paid;
                soldTicket.Ticket.AvailableCapacity -= soldTicket.Count;
                await unitOfWork.Repository<SoldTicket>().UpdateAsync(soldTicket);
                await mediator.Publish(new PaidTicketEvent() { SoldTicketId = item }, cancellationToken);
            }
            else
            {
                notAvailableTicket = true;
                ticketMessages.Add($"{soldTicket.Ticket.Title} not available");
            }
        } 
        ///////////////////////////////End Product Sold
        
        ///////////////////////Start Product Item 
        var soldProductsId = cart.CartItems.Where(p => p.CartItemType == CartItemType.Product)
            .Select(item => item.ProductSoldId.Value).ToList();

        foreach (var productSoldId in soldProductsId)
        {
            var productSold = await unitOfWork.Repository<ProductSold>()
                .GetByIdAsync(productSoldId);

            var cartItem = cart.CartItems.FirstOrDefault(p => p.ProductSoldId == productSold.Id);

            if (cartItem != null)
            {
                productSold.Income = cartItem.GetRealPrice;
                productSold.ShareFestivalIncome = (decimal) ((100 - siteCommission.ProductSalesCommission)/100) * productSold.Income;
            }

            productSold.Status = ProductSoldStatus.Paid;

            await unitOfWork.Repository<ProductSold>().UpdateAsync(productSold);
        }
        //////////////////////End Product Item

        //////////////////Start Special Account
        var specialAccountItem = cart.CartItems.Where(p => p.CartItemType == CartItemType.SpecialAccount);


        foreach (var spAccount in specialAccountItem)
        {
            var specialAccount = await unitOfWork.Repository<UserSpecialPeriod>()
                .GetByIdAsync(int.Parse(spAccount.ItemId));

            specialAccount.Status = UserSpecialAccountStatus.Open;
            specialAccount.OpenDateTime = DateTime.Now;
            switch (specialAccount.Period)
            {
                case StatusFeePeriod.Monthly:
                    specialAccount.CloseDateTime = DateTime.Now.AddMonths(1);
                    break;
                case StatusFeePeriod.ThreeMonth:
                    specialAccount.CloseDateTime = DateTime.Now.AddMonths(3);
                    break;
                case StatusFeePeriod.Yearly:
                    specialAccount.CloseDateTime = DateTime.Now.AddYears(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await userService.ChangeAccountStatus(FeeStatus.Special, specialAccount.UserId);
            await unitOfWork.Repository<UserSpecialPeriod>().UpdateAsync(specialAccount);
        }

        if (notAvailableTicket)
        {
            return await Result<CheckPaymentResponse>.FailAsync(ticketMessages);
        }
        
        ///////////End Special Account

        cart.CartDate = DateTime.Now;
        
        await unitOfWork.Repository<Cart>().UpdateAsync(cart);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await mediator.Publish(new CartPaidedEvent() { CartId = cart.Id });
        return await Result<CheckPaymentResponse>.SuccessAsync(new CheckPaymentResponse()
        {
            OrderId = cart.OrderId,
            PayerId =cart.PayerId,
            PaymentId = cart.PaymentId,
        });
    }
}
