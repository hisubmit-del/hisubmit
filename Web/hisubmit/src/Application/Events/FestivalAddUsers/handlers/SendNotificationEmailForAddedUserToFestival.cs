using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.RenderView;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Models.Emails;
using HiSubmit.Application.Requests.Mail;
using HiSubmit.Domain.Entities.Festivals;
using MediatR;

namespace HiSubmit.Application.Events.FestivalAddUsers.handlers;

public class SendNotificationEmailForAddedUserToFestival(
    IBackGroundJobService backGroundJobService,
    IUnitOfWork<int> unitOfWork,
    IMailService mailService,
    IUserService userService,
    IRenderViewService renderViewService)
    : INotificationHandler<FestivalAddUserEvent>
{
    public async Task Handle(FestivalAddUserEvent notification, CancellationToken cancellationToken)
    {
        backGroundJobService.AddEnqueue(()=>SendEmailForUser(notification.FestivalId,notification.UserId));
    }

    public async Task SendEmailForUser(int festivalId, string userId)
    {
        var festival = await unitOfWork.Repository<Festival>()
            .GetByIdAsync(festivalId);
        var user = await userService.GetAsync(userId);
        var model = new EmailNotificationAddUserToFestivalViewModel()
        {
            FestivalId = festival.Id,
            UserName = user.Data.FullName,
            FestivalName = festival.Name
        };
        var content =
            await renderViewService.RenderViewToStringAsync("_NotificationEmailAddUserToFestivalForUser", model);

       await mailService.SendAsync(new MailRequest()
        {
            Body = content,
            To = user.Data.Email,
            Subject = "You have been added to a festival"
        });
    }
}