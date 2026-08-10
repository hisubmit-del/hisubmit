using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Carts;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.ProductSold.AddProductSold.Handlers;

public class AddProductSoldToCartItem(ICartService cartService) : INotificationHandler<ProductSoldAddedEvent>
{
    public async Task Handle(ProductSoldAddedEvent notification, CancellationToken cancellationToken)
    {
        var cartItem = new AddToCartRequest
        {
            Price = notification.Price,
            Title = notification.ProductName,
            ImageUrl = notification.ProductImageUrl,
            ProductSoldId = notification.ProductSoldId,
            CartItemType = CartItemType.Product
        };
        await cartService.AddToCard(cartItem,cancellationToken);
    }
}