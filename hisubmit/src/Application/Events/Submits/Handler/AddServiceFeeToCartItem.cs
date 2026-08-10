using MediatR;
using System.Threading;
using HiSubmit.Domain.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Application.Interfaces.Carts;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;

namespace HiSubmit.Application.Events.Submits.Handler;

public class AddServiceFeeToCartItem(
    IUnitOfWork<int> unitOfWork,
    IUserService userService,
    ICartService cartService)
    : INotificationHandler<ProjectSubmitedEvent>
{
    public async Task Handle(ProjectSubmitedEvent notification, CancellationToken cancellationToken)
    {
        //service fee  is free for special user
        if(notification.FeeStatus==FeeStatus.Special || notification.Price==0)
            return;
        
        var feeSetting = await unitOfWork.Repository<SiteCommission>()
            .Entities.FirstOrDefaultAsync(cancellationToken);

        var serviceFee = notification.Price * (feeSetting.SubmitServiceFee / 100);
        if (serviceFee > feeSetting.MaximumServiceFee)
            serviceFee = feeSetting.MaximumServiceFee;
        
        if (serviceFee < feeSetting.MinimumServiceFee)
            serviceFee = feeSetting.MinimumServiceFee;

        await cartService.AddToCard(new AddToCartRequest()
        {
            Price =(decimal) serviceFee,
            Title = notification.Title,
            SubmitId =notification.SubmitId, 
            ImageUrl = notification.ImageUrl,
            CartItemType = CartItemType.ServiceFee,
            ItemId = notification.SubmitId.ToString(),
            Description = $"Service fee of submitting {notification.ProjectName} " +
                          $"to the {notification.FestivalName} festival",
        }, cancellationToken);
    }
}