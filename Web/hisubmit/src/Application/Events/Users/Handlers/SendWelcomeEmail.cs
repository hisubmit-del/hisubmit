using System;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.RenderView;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Application.Models.Emails;
using HiSubmit.Application.Requests.Mail;
using MediatR;

namespace HiSubmit.Application.Events.Users.Handlers;

//public class SendWelcomeEmail(
//    IBackGroundJobService backGroundJobService,
//    IMailService mailService,
//    IRenderViewService renderViewService)
//    : INotificationHandler<UserRegisteredEvent>
//{
//    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
//    {
//        var welcomeModel = new WelcomeViewModel()
//        {
//            FullName = notification.FullName
//        };

//        var mainContent =
//            await renderViewService.RenderViewToStringAsync("_WelcomeHisubmit", welcomeModel);

//        var backJob = backGroundJobService.AddEnqueue(() =>
//            mailService.SendAsync(new MailRequest()
//            {
//                Body = mainContent,
//                To = notification.Email,
//                Subject = "Welcome To Hisubmit.com"
//            }));
//    }
//}


public class SendConfirmedEmail(
    IBackGroundJobService backGroundJobService,
    IMailService mailService,
    IRenderViewService renderViewService)
    : INotificationHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var model = new ConfirmedEmailModel()
        {
            FullName = notification.FullName,
            VerificationCode = notification.VerificationUrl
        };

        var mainContent =
            await renderViewService.RenderViewToStringAsync("_ConfirmedEmail", model);

        var backJob = backGroundJobService.AddEnqueue(() =>
            mailService.SendAsync(new MailRequest()
            {
                Body = mainContent,
                To = notification.Email,
                Subject = "Welcome To Hisubmit.com"
            }));
    }
}
