using AutoMapper;
using System.Threading;
using HiSubmit.Domain.Enums;
using System.Threading.Tasks;
using HiSubmit.Domain.Entities;
using HiSubmit.Application.Models.Emails;
using HiSubmit.Application.Requests.Mail;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.RenderView;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;

namespace HiSubmit.Application.Services;

public interface INotificationService
{
    Task AddUserNotificationJob(AddUserNotificationRequest request);
    Task AddAdminNotificationJob(AddAdminNotificationRequest request);
    Task AddFestivalNotificationJob(AddFestivalNotificationRequest request);
}

public class NotificationService(
    IMapper mapper,
    IRenderViewService renderViewService,
    ISiteUrlService siteUrlService,
    IUserService userService,
    IUnitOfWork<int> unitOfWork,
    IMailService mailService,
    IBackGroundJobService backGroundJobService)
    : INotificationService
{
    public Task AddUserNotificationJob(AddUserNotificationRequest request)
    {
        backGroundJobService.AddEnqueue(() => AddUserNotification(request));
        return Task.CompletedTask;
    }

    public Task AddFestivalNotificationJob(AddFestivalNotificationRequest request)
    {
        backGroundJobService.AddEnqueue(() => AddFestivalNotification(request));
        return Task.CompletedTask;
    }

    public Task AddAdminNotificationJob(AddAdminNotificationRequest request)
    {
        backGroundJobService.AddEnqueue(() => AddAdminNotification(request));
        return Task.CompletedTask;
    }

    public async Task AddUserNotification(AddUserNotificationRequest request)
    {
        var user = await userService.GetAsync(request.UserId);
        var notification = mapper.Map<Notification>(request);
        notification.SiteAccountType = SiteAccountType.User;
        notification.Link = GenerateAbsolutUrl(notification.Link);
        await unitOfWork.Repository<Notification>()
            .AddAsync(notification);

        await unitOfWork.SaveChangesAsync(CancellationToken.None);

        var model = new PublicNotificationViewModel()
        {
            Link = request.Link,
            Title = request.Title
        };

        var content = await renderViewService
            .RenderViewToStringAsync("_PublicNotificationEmail", model);

        var mailRequest = new MailRequest()
        {
            Body = content,
            To = user.Data.Email,
            Subject = request.Title,
        };

        await mailService.SendAsync(mailRequest);
    }


    public async Task AddFestivalNotification(AddFestivalNotificationRequest request)
    {
        var festival = await unitOfWork.Repository<Festival>()
            .GetByIdAsync(request.FestivalId);

        var notification = mapper.Map<Notification>(request);
        notification.SiteAccountType = SiteAccountType.Festival;
        notification.Link = GenerateAbsolutUrl(notification.Link);
        await unitOfWork.Repository<Notification>().AddAsync(notification);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);

        var model = new PublicNotificationViewModel()
        {
            Link = request.Link,
            Title = request.Title
        };

        var content = await renderViewService
            .RenderViewToStringAsync("_PublicNotificationEmail", model);
        var mailRequest = new MailRequest()
        {
            Body = content,
            To = festival.Email,
            Subject = request.Title,
        };
        await mailService.SendAsync(mailRequest);
    }

    public async Task AddAdminNotification(AddAdminNotificationRequest request)
    {
        var admins = await userService.GetAllAdminUsers();
        var notification = mapper.Map<Notification>(request);
        notification.SiteAccountType = SiteAccountType.Admin;
        notification.Link = GenerateAbsolutUrl(notification.Link);
        await unitOfWork.Repository<Notification>().AddAsync(notification);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);

        var model = new PublicNotificationViewModel()
        {
            Link = request.Link,
            Title = request.Title
        };

        var content = await renderViewService
            .RenderViewToStringAsync("_PublicNotificationEmail", model);
        foreach (var admin in admins)
        {
            var mailRequest = new MailRequest()
            {
                Body = content,
                To = admin.Email,
                Subject = request.Title,
            };
            await mailService.SendAsync(mailRequest);
        }
    }

    private string GenerateAbsolutUrl(string relativeUrl)
    {
        // if (relativeUrl[0] != '/')
        //     relativeUrl = "/" + relativeUrl;

        return $"{siteUrlService.GetBaseUrl()}{relativeUrl}";
    }
}