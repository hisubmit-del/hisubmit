using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.News.Commands;

public class DeleteNewCommand:IRequest<IResult>
{
    public  int Id { get; set; }
}

public class DeleteNewCommandHandler : IRequestHandler<DeleteNewCommand, IResult>
{
    private IStringLocalizer<DeleteNewCommandHandler> _localizer;
    private readonly IUnitOfWork<int> _unitOfWork;

    public DeleteNewCommandHandler(IStringLocalizer<DeleteNewCommandHandler> localizer, IUnitOfWork<int> unitOfWork)
    {
        _localizer = localizer;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult> Handle(DeleteNewCommand request, CancellationToken cancellationToken)
    {
        var newDb = await _unitOfWork.Repository<New>().GetByIdAsync(request.Id);
        if (newDb == null) return await Result.FailAsync(_localizer["new not found"]);
        await _unitOfWork.Repository<New>().DeleteAsync(newDb);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(_localizer["New Deleted"]);
    }
}