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

namespace HiSubmit.Application.Events.RefereeSubmitJudgingForProject.Handlers;

public class SendRefereeSubmitJudgingForProjectEmail :
    INotificationHandler<RefereeSubmitJudgingFroProjectEvent>
{
    private readonly IMailService _mailService;
    private readonly IUserService _userService;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IRenderViewService _renderViewService;
    private readonly IBackGroundJobService _backGroundJobService;

    public SendRefereeSubmitJudgingForProjectEmail
    (IRenderViewService renderViewService, 
        IMailService mailService,
        IUnitOfWork<int> unitOfWork, 
        IBackGroundJobService backGroundJobService, 
        IUserService userService)
    {
        _renderViewService = renderViewService;
        _mailService = mailService;
        _unitOfWork = unitOfWork;
        _backGroundJobService = backGroundJobService;
        _userService = userService;
    }

    public async Task Handle(RefereeSubmitJudgingFroProjectEvent notification,
        CancellationToken cancellationToken)
    {
        _backGroundJobService.AddEnqueue(() => SendMail(notification.ProjectJudgingId));
    }

    public async Task SendMail(int projectJudgingId)
    {
        var projectJudging = await _unitOfWork.Repository<ProjectJudging>()
            .Entities.Where(p => p.Id == projectJudgingId)
            .Include(p => p.Submit).ThenInclude(p => p.Festival)
            .Include(p => p.Submit).ThenInclude(p => p.Project)
            .FirstOrDefaultAsync();

        var referee = await _userService.GetAsync(projectJudging.UserId);

         const string emailViewModel = "The result of the arbitration was recorded for a project";
        var model = new RefreeSubmitJudgingForProjectEmailViewModel()
        {
            Title = emailViewModel,
            RefereeFullName = referee.Data.FullName,
            ProjectJudgingId = projectJudging.SubmitId,
            FestivalId = projectJudging.Submit.FestivalId,
            ProjectTitle = projectJudging.Submit.Project.Title,
        };
        var content = await _renderViewService
            .RenderViewToStringAsync("_RefereeSubmitJudgingFroProject", model);
        var request = new MailRequest()
        {
            Body = content,
            Subject = emailViewModel,
            To = projectJudging.Submit.Festival.Email,
        };
        await _mailService.SendAsync(request);
    }
}