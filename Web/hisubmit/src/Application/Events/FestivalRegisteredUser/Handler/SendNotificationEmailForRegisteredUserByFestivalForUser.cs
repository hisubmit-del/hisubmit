using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.RenderView;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Application.Models.Emails;
using HiSubmit.Application.Requests.Mail;
using HiSubmit.Domain.Entities.Festivals;
using MediatR;

namespace HiSubmit.Application.Events.FestivalRegisteredUser.Handler;

public class SendNotificationEmailForRegisteredUserByFestivalForUser(
    IBackGroundJobService backGroundJobService,
    IUnitOfWork<int> unitOfWork,
    IMailService mailService,
    IRenderViewService renderViewService)
    : INotificationHandler<FestivalRegisteredUserEvent>
{
    public async Task Handle(FestivalRegisteredUserEvent notification, CancellationToken cancellationToken)
    {
        backGroundJobService.AddEnqueue(() => SendEmailForUser(notification.FestivalId, notification.FullName,
            notification.Email, notification.Password));
    }
    
    
    public async Task SendEmailForUser(int festivalId,string fullName,string email,string password)
    {
        var festival = await unitOfWork.Repository<Festival>()
            .GetByIdAsync(festivalId);
        var model = new NotificationEmailRegisteredUserByFestivalForUserViewModel()
        {
            FestivalId = festival.Id,
            FullName =fullName ,
            FestivalName = festival.Name,
            Password =password,
            Email = email
        };
        var content =
            await renderViewService.RenderViewToStringAsync("_NotificationEmailRegisteredUserToFestivalForUser", model);

        await mailService.SendAsync(new MailRequest()
        {
            Body = content,
            To = email,
            Subject = "Your email has been added to a festival"
        });
    }
}