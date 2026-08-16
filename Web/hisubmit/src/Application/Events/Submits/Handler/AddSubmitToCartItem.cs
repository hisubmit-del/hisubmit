using MediatR;
using System.Threading;
using HiSubmit.Domain.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiSubmit.Application.Extensions;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Payments;

namespace HiSubmit.Application.Events.Submits.Handler;

public class AddSubmitToCartItem(IUnitOfWork<int> unitOfWork) 
    : INotificationHandler<ProjectSubmitedEvent>
{
    public async Task Handle(ProjectSubmitedEvent notification, CancellationToken cancellationToken)
    {
        var specification = new GetOpenCartUserSpecification(notification.UserId);

        var cart = await unitOfWork.Repository<Cart>()
            .Entities
            .Specify(specification)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = notification.UserId
            };
            await unitOfWork.Repository<Cart>().AddAsync(cart);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var cartItem = new CarTItem
        {
            CartId = cart.Id,
            Title = notification.Title,
            SubmitId =notification.SubmitId, 
            ImageUrl = notification.ImageUrl,
            CartItemType = CartItemType.Submit,
            Price = (decimal)notification.Price,
            ItemId = notification.SubmitId.ToString(),
            Description =  $"Cost of submitting {notification.ProjectName} to the {notification.FestivalName} festival",
        };

        await unitOfWork.Repository<CarTItem>().AddAsync(cartItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

