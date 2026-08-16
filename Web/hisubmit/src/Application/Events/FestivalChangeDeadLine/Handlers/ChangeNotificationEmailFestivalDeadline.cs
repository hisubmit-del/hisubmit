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

namespace HiSubmit.Application.Events.FestivalChangeDeadLine.Handlers;

public class ChangeNotificationEmailFestivalDeadline(
    IBackGroundJobService backGroundJobService,
    IMailService mailService,
    IRenderViewService renderViewService,
    IUnitOfWork<int> unitOfWork)
    : INotificationHandler<FestivalChangeDeadlineEvent>
{
    public async Task Handle(FestivalChangeDeadlineEvent notification, CancellationToken cancellationToken)
    {
        var festival = notification.Festival;
        backGroundJobService.Delete(festival.SendEventStartDateEmailJobId, festival.SendOpenDateEmailJobId,
            festival.SendEventEndDateEmailJobId, festival.SendNotificationDateEmailJobId);
        
        festival.SendNotificationDateEmailJobId = await SendNotificationDateEmail(festival);
        festival.SendOpenDateEmailJobId = await SendOpenDateNotificationEmail(festival);
        festival.SendEventEndDateEmailJobId = await SendEventEndDateNotificationEmail(festival);
        festival.SendEventStartDateEmailJobId = await SendEventStartDateNotificationEmail(festival);
        await unitOfWork.Repository<Festival>().UpdateAsync(festival);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> SendNotificationDateEmail(Festival festival)
    {
        var model = new SendNotificationDateEmailViewModel()
        {
            NotificationDate = festival.NotificationDate.Value,
            FestivalName = festival.Name,
            Title = "Today is the notification day of the festival",
            FestivalId = festival.Id
        };
        var content = await renderViewService.RenderViewToStringAsync("_NotificationDateEmail", model);
        var request = new MailRequest()
        {
            Body = content,
            To = festival.Email,
            Subject = "Today is the notification day of the festival"
        };
        if (festival.NotificationDate == null) return string.Empty;
        
        var jobId = backGroundJobService
            .AddSchedule(() => SendEmail(request), festival.NotificationDate.Value);
        
        return jobId;
    }

    public async Task<string> SendOpenDateNotificationEmail(Festival festival)
    {
        var model = new SendOpenDateNotificationEmailViewModel();
        var content = await renderViewService.RenderViewToStringAsync("_OpenDateNotificationEmail", model);
        var request = new MailRequest()
        {
            Body = content,
            To = festival.Email,
            Subject = "Today is the opening day of the festival"
        };
        if (festival.NotificationDate == null) return string.Empty;
        var jobId = backGroundJobService
            .AddSchedule(() => SendEmail(request), festival.OpeningDate.Value);
        return jobId;
    }

    public async Task<string> SendEventEndDateNotificationEmail(Festival festival)
    {
        var model = new SendEventEndDateNotificationEmailViewModel();
        var content = await renderViewService.RenderViewToStringAsync("_EventEndDateNotificationEmail", model);
        var request = new MailRequest()
        {
            Body = content,
            To = festival.Email,
            Subject = "Today  is the last day of the festival "
        };
        if (festival.NotificationDate == null) return string.Empty;
        var jobId = backGroundJobService
            .AddSchedule(() => SendEmail(request), festival.EventEndDate.Value);
        return jobId;
    }

    public async Task<string> SendEventStartDateNotificationEmail(Festival festival)
    {
        var model = new SendEventStartDateNotificationEmailViewModel();
        var content = await renderViewService.RenderViewToStringAsync("_EventStartDateNotificationEmail", model);
        var request = new MailRequest()
        {
            Body = content,
            To = festival.Email,
            Subject = "Today is the start of the festival"
        };
        if (festival.NotificationDate == null) return string.Empty;
        var jobId = backGroundJobService
            .AddSchedule(() => SendEmail(request), festival.EventStartDate.Value);
        return jobId;
    }

    public async Task SendEmail(MailRequest request)
    {
        await mailService.SendAsync(request);
    }
}
