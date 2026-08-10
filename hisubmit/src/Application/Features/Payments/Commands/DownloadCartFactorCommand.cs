using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Events.Payments.PaidCartEvent;
using HiSubmit.Application.Interfaces.PdfConverter;
using HiSubmit.Application.Interfaces.RenderView;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Features.Payments.Commands;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Payments.Commands;

public class DownloadCartFactorCommand : DownloadCartFactorRequest, IRequest<IResult<DownloadCartFactorResponse>>;

public class
    DownloadCartFactorCommandHandler(
        IRenderViewService renderViewService,
        IPdfGenerator pdfGenerator,
        IMediator mediator,
        IUnitOfWork<int> unitOfWork,
        IMapper mapper)
    : IRequestHandler<DownloadCartFactorCommand, IResult<DownloadCartFactorResponse>>
{
    public async Task<IResult<DownloadCartFactorResponse>>
        Handle(DownloadCartFactorCommand request, CancellationToken cancellationToken)
    {
        var cart = await unitOfWork.Repository<Cart>().Entities
            .Where(p => p.Id == request.Id)
            .Include(p => p.CartItems)
            .Include(p => p.CartItems).ThenInclude(p => p.Submit).ThenInclude(p => p.Festival)
            .Include(p => p.CartItems).ThenInclude(p => p.Submit)
            .ThenInclude(p => p.SubmitDeadlineEventCategories).ThenInclude(p => p.DeadlineEventCategory)
            .ThenInclude(p => p.EventCategory)
            .Include(p => p.CartItems).ThenInclude(p => p.ProductSold).ThenInclude(p => p.Product)
            .ThenInclude(p => p.Festival)
            .Include(p => p.CartItems).ThenInclude(p => p.SoldTicket).ThenInclude(p => p.Ticket)
            .ThenInclude(p => p.Venue).ThenInclude(p => p.Festival)
            .ProjectTo<GetAllCartsResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart == null)
            return await Result<DownloadCartFactorResponse>.FailAsync("Cart not found");


        var content = await renderViewService.RenderViewToStringAsync("CartFactor", cart, "Tickets");
        var pdfFileByteArray = await pdfGenerator.GenerateFile(new PdfGeneratorRequest()
        {
            Content = content,
            DocTitle = $"cartFile.pdf"
        });
        await mediator.Publish(new CartPaidedEvent() { CartId = cart.Id });
        return await Result<DownloadCartFactorResponse>.SuccessAsync(new DownloadCartFactorResponse()
        {
            File = pdfFileByteArray,
            MimeType = ApplicationConstants.MimeTypes.Pdf,
            FileName = $"cartFile.pdf"
        });
    }
}