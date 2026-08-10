using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Features.SoldTickets.Commands;
using HiSubmit.Application.Interfaces.PdfConverter;
using HiSubmit.Application.Interfaces.RenderView;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Models.Tickets;
using HiSubmit.Application.Requests.Mail;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Events.TicketsSold.Handlers;

public class CreateBadgeFileHandler:INotificationHandler<PaidBadgeEvent>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<DownloadTicketsFileQueryHandler> _localizer;
    private readonly IRenderViewService _renderViewService;
    private readonly IPdfGenerator _pdfGeneratorl;
    private readonly IUserService _userService;
    private readonly IBackGroundJobService _backGroundJobService;
    private readonly IMailService _mailService;

    public CreateBadgeFileHandler
    (IUnitOfWork<int> unitOfWork, IStringLocalizer<DownloadTicketsFileQueryHandler> localizer,
        IRenderViewService renderViewService, IPdfGenerator pdfGeneratorl, IUserService userService,
        IBackGroundJobService backGroundJobService, IMailService mailService)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
        _renderViewService = renderViewService;
        _pdfGeneratorl = pdfGeneratorl;
        _backGroundJobService = backGroundJobService;
        _userService = userService;
        _mailService = mailService;
    }
    
    
    public async Task Handle(PaidBadgeEvent notification, CancellationToken cancellationToken)
    {
        _backGroundJobService.AddEnqueue(() =>
            CreateFile(notification, cancellationToken)
        );
    }
    
    public async Task CreateFile(PaidBadgeEvent notification, CancellationToken cancellationToken)
    {
        var soldTicket = await _unitOfWork.Repository<SoldTicket>()
            .Entities.Include(p => p.ShowTime)
            .Include(p => p.Ticket).ThenInclude(p=>p.Venue)
            .FirstOrDefaultAsync(p => p.Id == notification.SoldTicketId, cancellationToken);
        var user = await _userService.GetAsync(soldTicket.UserId);
        if (soldTicket is not { SoldTicketStatus: SoldTicketStatus.Paid })
        {
            throw new ApplicationException();
        }

        var email = soldTicket.ForOtherUser ? soldTicket.OtherUserEmail : user.Data.Email;

        var model = new TicketViewModel()
        {
            Email = email,
            GuidString = soldTicket.SerialNumber.ToString(),
            QRCode = soldTicket.QrCode,
            TicketTitle = soldTicket.Ticket.Title,
            ShowTimeName = soldTicket.Ticket.Venue.Name,
            Count = soldTicket.Count
        };
        var content = await _renderViewService.RenderViewToStringAsync("Badge", model, "Tickets");
        var pdfFileByteArray = await _pdfGeneratorl.GenerateFile(new PdfGeneratorRequest()
        {
            Content = content,
            DocTitle = $"{soldTicket.Ticket.Title}.pdf"
        });
        soldTicket.PdfFile = pdfFileByteArray;
        await _unitOfWork.Repository<SoldTicket>().UpdateAsync(soldTicket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var emailContent = await _renderViewService.RenderViewToStringAsync("_TicketSealed");
        var mailRequest = new MailRequest()
        {
            Body = emailContent,
            Subject = "Ticket",
            To = email,
            Attachments = new List<MailAttachment>()
                { new MailAttachment() 
                    { File = soldTicket.PdfFile, Name = $"{soldTicket.Ticket.Title}.pdf"}}
        };
        await  _mailService.SendAsync(mailRequest);
    }

}