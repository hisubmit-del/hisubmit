using AutoMapper;
using System.Threading;
using HiSubmit.Domain.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiSubmit.Application.Extensions;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Payments;

namespace HiSubmit.Application.Interfaces.Carts;

public interface ICartService
{
    Task<IResult> AddToCard(AddToCartRequest request,CancellationToken cancellationToken);
}

public class CartService(ICurrentUserService currentUserService, IUnitOfWork<int> unitOfWork, IMapper mapper)
    : ICartService
{
    public async Task<IResult> AddToCard(AddToCartRequest request,CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var specification = new GetOpenCartUserSpecification(userId);

        
        var cart = await unitOfWork.Repository<Cart>()
            .Entities
            .Specify(specification)
            .FirstOrDefaultAsync(cancellationToken);

        
        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId
            };
            await unitOfWork.Repository<Cart>().AddAsync(cart);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var cartItem = mapper.Map<CarTItem>(request);

        cartItem.CartId = cart.Id;

        await unitOfWork.Repository<CarTItem>().AddAsync(cartItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}
public class AddToCartRequest
{
    public string Title { get; set; }
    public string ItemId { get; set; }
    public decimal Price { get; set; }

    public  string Description { get; set; }
    public  string ImageUrl { get; set; }
    
    public  int? SubmitId { get; set; }

    public int? ProductSoldId { get; set; }
    
    public  int? SoldTicketId { get; set; }
    
    public CartItemType CartItemType { get; set; }
}
