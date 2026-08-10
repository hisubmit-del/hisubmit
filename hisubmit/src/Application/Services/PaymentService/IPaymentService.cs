using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.Submits.PaidSubmit;
using HiSubmit.Application.Events.TicketsSold;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Services.PaymentService;

public interface IPaymentService
{
    Task<Result<int>> ChangeCartItemState(Cart cart,CancellationToken cancellationToken);

}

public class PaymentService(IMediator mediator, IUnitOfWork<int> unitOfWork,IUserService userService) : IPaymentService
{
    public async Task<Result<int>> ChangeCartItemState(Cart cart, CancellationToken cancellationToken)
    {
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

        var soldTicketsId = cart.CartItems.Where(p => p.CartItemType == CartItemType.Ticket)
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
            return await Result<int>.FailAsync(ticketMessages);
        }

        return await Result<int>.SuccessAsync();

    }
}