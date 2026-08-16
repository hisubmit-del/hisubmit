using System;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.RenderView;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Application.Models.Emails;
using HiSubmit.Application.Requests.Mail;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;

namespace HiSubmit.Application.Features.Emails;

public class EmailSender : IRequest<IResult>
{

}

public class EmailSenderHandler : IRequestHandler<EmailSender, IResult>
{
    private readonly IBackGroundJobService _backGroundJobService;
    private readonly IMailService _mailService;
    private readonly IRenderViewService _renderViewService;

    public EmailSenderHandler(IBackGroundJobService backGroundJobService,
        IMailService mailService, IRenderViewService renderViewService)
    {
        _backGroundJobService = backGroundJobService;
        _mailService = mailService;
        _renderViewService = renderViewService;
    }

    public async Task<IResult> Handle(EmailSender request, CancellationToken cancellationToken)
    {
        // var welcomeModel = new WelcomeViewModel()
        // {
        //     FullName = "User"
        // };
        var model = new NotificationEmailRegisteredUserByFestivalForUserViewModel()
        {
            FestivalName = "AmirFestival",
            FestivalId = 2,
            FullName = "Amir Mohammadi",
            Password = "123edrf",
            Email = "amyrm1960@gmail.com"
        };
        var model2 = new EmailNotificationAddUserToFestivalViewModel()
        {
            FestivalName = "AmirFestival",
            FestivalId = 2,
            UserName = "Amir Mohammadi"
        };
        var toEmail = "belal.taheri@gmail.com";
        
        var model3 = new WelcomeViewModel();
        var maincontent3 = await _renderViewService.RenderViewToStringAsync("_WelcomeHisubmit", model3);
        var mainContent =
            await _renderViewService.RenderViewToStringAsync("_NotificationEmailAddUserToFestivalForUser", model2);
        var backJob = _backGroundJobService.AddEnqueue(() =>
            _mailService.SendAsync(new MailRequest()
            {
                Body = mainContent,
                To = toEmail,
                Subject = "New Submission"
            }));
        var mainContent2 =
            await _renderViewService.RenderViewToStringAsync("_NotificationEmailRegisteredUserToFestivalForUser",
                model);

        var backJob2 = _backGroundJobService.AddEnqueue(() =>
            _mailService.SendAsync(new MailRequest()
            {
                Body = mainContent2,
                To =toEmail,
                Subject = "New Submission"
            }));


        var backJob3 = _backGroundJobService.AddEnqueue(() =>
            _mailService.SendAsync(new MailRequest()
            {
                Body = maincontent3,
                To = toEmail,
                Subject = "Welcome To HiSubmit"
            }));


        var model4 = new RefreeSubmitJudgingForProjectEmailViewModel()
        {
            FestivalId = 1,
            Title = "The result of the arbitration was recorded for a project",
            ProjectTitle = "Project Name ",
            RefereeFullName = "Amir Mohammadi",
            ProjectJudgingId = 2
        };

        var mainContent4 = await _renderViewService.RenderViewToStringAsync("_RefereeSubmitJudgingFroProject",model4);
        var backJob4 = _backGroundJobService.AddEnqueue(() =>
            _mailService.SendAsync(new MailRequest()
            {
                Body = mainContent4,
                To = toEmail,
                Subject = "Welcome To Hisubmit"
            }));


        var model5 = new RefereeAddToProjectEmailViewModel()
        {
            FestivalName = "Festival Name",
            Email = "festivalEmail@gmail.com",
            ProjectTitle = "Project Title",
            Title = "A new project has been assigned to you"
        };
        var mainContetn5 = await _renderViewService.RenderViewToStringAsync("_RefereeAddToProjectEmail",model5);
        var backJob5 = _backGroundJobService.AddEnqueue(() =>
            _mailService.SendAsync(new MailRequest()
            {
                Body = mainContent4,
                To = toEmail,
                Subject = "Welcome To HiSubmit"
            }));
        
        
        return await Result.SuccessAsync();
    }
}
