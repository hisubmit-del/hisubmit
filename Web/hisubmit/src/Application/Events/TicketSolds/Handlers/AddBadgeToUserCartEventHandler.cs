using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Payments;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Events.TicketsSold.Handlers;

public class AddBadgeToUserCartEventHandler(IUnitOfWork<int> unitOfWork)
    : INotificationHandler<BadgeSoldEvent>
{
    public async Task Handle(BadgeSoldEvent notification, CancellationToken cancellationToken)
    {
        var badge = await unitOfWork.Repository<SoldTicket>()
            .Entities.Include(p=>p.Ticket).ThenInclude(p=>p.Venue)
            .ThenInclude(p=>p.Festival)
            .FirstOrDefaultAsync(p=>p.Id==notification.TicketSoldId,cancellationToken);

        var specification = new GetOpenCartUserSpecification(badge.UserId);

        
        var cart = await unitOfWork.Repository<Cart>()
            .Entities
            .Specify(specification)
            .FirstOrDefaultAsync(cancellationToken);

        
        if (cart == null)
        {
            cart = new Cart()
            {
                UserId = badge.UserId
            };
            await unitOfWork.Repository<Cart>().AddAsync(cart);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var cartItem = new CarTItem()
        {
            ItemId = badge.Id.ToString(),
            CartId = cart.Id,
            Title = $"Badge: {badge.Ticket.Title}",
            Price = badge.Cost * badge.Count,
            SoldTicketId = badge.Id,
            ImageUrl = badge.Ticket.Venue.Festival.LogoURL,
            CartItemType = CartItemType.Badge
        };

        await unitOfWork.Repository<CarTItem>().AddAsync(cartItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}