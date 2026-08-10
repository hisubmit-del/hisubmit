using System.Linq;
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
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Events.RefeerrAddToProjects.Handlers;

public class SendEmailRefereeAddToProjectsHandler:INotificationHandler<RefereeAddToProjectsEvent>
{
    private readonly IRenderViewService _renderViewService;
    private readonly IMailService _mailService;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IBackGroundJobService _backGroundJobService;
    private readonly IUserService _userService;
    public SendEmailRefereeAddToProjectsHandler
        (IRenderViewService renderViewService, IMailService mailService,IUserService userService,
            IUnitOfWork<int> unitOfWork, IBackGroundJobService backGroundJobService)
    {
        _renderViewService = renderViewService;
        _mailService = mailService;
        _unitOfWork = unitOfWork;
        _backGroundJobService = backGroundJobService;
        _userService = userService;
    }

    public async Task Handle(RefereeAddToProjectsEvent notification, CancellationToken cancellationToken)
    {
        _backGroundJobService.AddEnqueue(() => SendEmail(notification.ProjectJudgingId));
    }

    public async Task SendEmail(int projectJudgingId)
    {
        var projectJudging = await _unitOfWork.Repository<ProjectJudging>()
            .Entities.Where(p=>p.Id==projectJudgingId)
            .Include(p=>p.Submit).ThenInclude(p=>p.Project)
            .Include(p=>p.Submit).ThenInclude(p=>p.Festival)
            .FirstOrDefaultAsync();

        var user =await _userService.GetAsync(projectJudging.UserId);

        const string emailTitle = "A new project has been assigned to you";
        var model = new RefereeAddToProjectEmailViewModel()
        {
            FestivalId = projectJudging.Submit.FestivalId,
            FestivalName = projectJudging.Submit.Festival.Name,
            ProjectTitle = projectJudging.Submit.Project.Title,
            ProjectJudgingId = projectJudgingId,
            Title = emailTitle
        };

        var content = await _renderViewService.RenderViewToStringAsync("_RefereeAddToProjectEmail", model);
        var mailRequest = new MailRequest()
        {
            Body = content,
            Subject = emailTitle,
            To = user.Data.Email
        };
        await _mailService.SendAsync(mailRequest);
    }
}

