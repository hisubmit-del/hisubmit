using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.RenderView;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests.Mail;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Domain.Entities.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Events.Payments.PaidCartEvent.Handlers;

public class SendNotifyEmailPaidCart(
    IMapper mapper,
    IUserService userService,
    IMailService _mailService,
    IUnitOfWork<int> _unitOfWork,
    IRenderViewService _renderViewService,
    IBackGroundJobService _backGroundJobService)
    : INotificationHandler<CartPaidedEvent>
{
    public  Task Handle(CartPaidedEvent notification, CancellationToken cancellationToken)
    {
        _backGroundJobService.AddEnqueue(() => SendEmail(notification));
        return Task.CompletedTask;
    }

    public async Task SendEmail(CartPaidedEvent notification)
    {
        var cart = await _unitOfWork.Repository<Cart>().Entities
            .Where(p => p.Id == notification.CartId)
            .Include(p => p.CartItems)
            .Include(p => p.CartItems).ThenInclude(p => p.Submit).ThenInclude(p => p.Festival)
            .Include(p => p.CartItems).ThenInclude(p => p.Submit)
            .ThenInclude(p => p.SubmitDeadlineEventCategories).ThenInclude(p => p.DeadlineEventCategory)
            .ThenInclude(p => p.EventCategory)
            .Include(p => p.CartItems).ThenInclude(p => p.ProductSold).ThenInclude(p => p.Product)
            .ThenInclude(p => p.Festival)
            .Include(p => p.CartItems).ThenInclude(p => p.SoldTicket).ThenInclude(p => p.Ticket)
            .ThenInclude(p => p.Venue).ThenInclude(p => p.Festival)
            .ProjectTo<GetAllCartsResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(CancellationToken.None);

        if (cart == null)
            throw new Exception("Cart not found");
        var user =await userService.GetAsync(cart.UserId);
        cart.UserFullName = user.Data.FullName;
        var content = await _renderViewService
            .RenderViewToStringAsync("_CartFactor", cart);
        // var builer=new BodyBuilder();
        // var image = builder.LinkedResources.Add(@"path/to/your/image.png");
       // image.ContentId = "logo";
        var mailRequest = new MailRequest()
        {
            Body = content,
            To = user.Data.Email,
            Subject = "Payment Information",
            
        };
        await _mailService.SendAsync(mailRequest);
    }
}