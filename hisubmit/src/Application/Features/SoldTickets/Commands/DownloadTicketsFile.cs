using System;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.PdfConverter;
using HiSubmit.Application.Interfaces.RenderView;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Models.Tickets;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.SoldTickets.Commands;

public class DownloadTicketsFileQuery : IRequest<IResult<DownloadTicketFileResponse>>
{
    public int SoldTicketId { get; set; }
}

public class DownloadTicketsFileQueryHandler : 
IRequestHandler<DownloadTicketsFileQuery,IResult<DownloadTicketFileResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<DownloadTicketsFileQueryHandler> _localizer;
    private readonly IRenderViewService _renderViewService;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IUserService _userService;

    public DownloadTicketsFileQueryHandler
    (IUnitOfWork<int> unitOfWork, IStringLocalizer<DownloadTicketsFileQueryHandler> localizer,
        IRenderViewService renderViewService, IPdfGenerator pdfGenerator, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
        _renderViewService = renderViewService;
        _pdfGenerator = pdfGenerator;
        _userService = userService;
    }


    public async Task<IResult<DownloadTicketFileResponse>> Handle(DownloadTicketsFileQuery request,
        CancellationToken cancellationToken)
    {
        // var soldTicket = await _unitOfWork.Repository<SoldTicket>()
        //     .GetByIdAsync(request.SoldTicketId);
        var soldTicket = await _unitOfWork.Repository<SoldTicket>()
            .Entities.Include(p => p.ShowTime)
            .Include(p=>p.Ticket)
            .FirstOrDefaultAsync(p => p.Id == request.SoldTicketId, cancellationToken);
        //
        // var user = await _userService.GetAsync(soldTicket.UserId);
        // if (soldTicket is not { SoldTicketStatus: SoldTicketStatus.Paid })
        // {
        //     throw new ApplicationException();
        // }
        //
        // var email = soldTicket.ForOtherUser ? soldTicket.OtherUserEmail : user.Data.Email;
        //
        // var model = new TicketViewModel()
        // {
        //     Email = email,
        //     EndDate = soldTicket.ShowTime.CloseDate,
        //     StartDate = soldTicket.ShowTime.OpenDate,
        //     GuidString = soldTicket.SerialNumber.ToString(),
        //     QRCode = soldTicket.QrCode,
        //     TicketTitle =soldTicket.Ticket.Title,
        //     ShowTimeName = soldTicket.ShowTime.Name
        // };
        // var content = await _renderViewService.RenderViewToStringAsync("Tickets", model, "Tickets");
        // var pdfFileByteArray = await _pdfGeneratorl.GenerateFile(new PdfGeneratorRequest()
        // {
        //     Content = content,
        //     DocTitle = $"{soldTicket.Ticket.Title}.pdf"
        // });

        var response = new DownloadTicketFileResponse()
        {
            File = soldTicket.PdfFile,
            MimeType = ApplicationConstants.MimeTypes.Pdf,
            FileName = $"{soldTicket.Ticket.Title}.pdf"
        };
        return await  Result<DownloadTicketFileResponse>.SuccessAsync(response);
    }
}

public class DownloadTicketFileResponse
{
    public string MimeType { get; set; } 
    public string FileName { get; set; }
    public byte[] File { get; set; }
}