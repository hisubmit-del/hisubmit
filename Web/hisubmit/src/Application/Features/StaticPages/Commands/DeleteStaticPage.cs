using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.StaticPages.Commands;

public class DeleteStaticPageCommand:IRequest<IResult>
{
    public  int Id { get; set; }
}

public class DeleteStaticPageCommandHandler(
    IStringLocalizer<DeleteStaticPageCommandHandler> localizer,
    IUnitOfWork<int> unitOfWork)
    : IRequestHandler<DeleteStaticPageCommand, IResult>
{
    public async Task<IResult> Handle(DeleteStaticPageCommand request, CancellationToken cancellationToken)
    {
        var newDb = await unitOfWork.Repository<StaticPageAndFAQ>().GetByIdAsync(request.Id);
        if (newDb == null) return await Result.FailAsync(localizer["static page not found"]);
        await unitOfWork.Repository<StaticPageAndFAQ>().DeleteAsync(newDb);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(localizer["static page Deleted"]);
    }
}