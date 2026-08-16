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

namespace  HiSubmit.Application.Events.TicketsSold.Handlers;

public class AddTicketToUserCardHandler(IUnitOfWork<int> unitOfWork)
    : INotificationHandler<TicketSoldEvent>
{
    public async Task Handle(TicketSoldEvent notification, CancellationToken cancellationToken)
    {
        var ticket = await unitOfWork.Repository<SoldTicket>()
            .Entities.Include(p=>p.Ticket).ThenInclude(p=>p.Venue)
            .ThenInclude(p=>p.Festival)
            .Include(p=>p.ShowTime)

            .FirstOrDefaultAsync(p=>p.Id==notification.TicketSoldId,cancellationToken);

        var specification = new GetOpenCartUserSpecification(ticket.UserId);

        var cart = await unitOfWork.Repository<Cart>()
            .Entities
            .Specify(specification)
            .FirstOrDefaultAsync(cancellationToken);

        
        if (cart == null)
        {
            cart = new Cart()
            {
                UserId = ticket.UserId
            };
            await unitOfWork.Repository<Cart>().AddAsync(cart);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var cartItem = new CarTItem()
        {
            ItemId = ticket.Id.ToString(),
            CartId = cart.Id,
            ImageUrl = ticket.Ticket.Venue.Festival.LogoURL,
            Title = $"Ticket: {ticket.Ticket.Title} -{ticket.ShowTime.Name}",
            Price = ticket.Cost,
            CartItemType = CartItemType.Ticket,
            SoldTicketId = ticket.Id,
        };

        await unitOfWork.Repository<CarTItem>().AddAsync(cartItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

