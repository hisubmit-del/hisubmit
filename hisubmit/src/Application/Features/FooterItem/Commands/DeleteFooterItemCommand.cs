using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.FooterItems.Commands;

public class DeleteFooterItemCommand:IRequest<IResult>
{
    public int Id { get; set; }
}

public class DeleteFooterItemCommandHandler : IRequestHandler<DeleteFooterItemCommand, IResult>
{
    private readonly IStringLocalizer<DeleteFooterItemCommandHandler> _localizer;
    private readonly IUnitOfWork<int> _unitOfWork;

    public DeleteFooterItemCommandHandler
        (IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteFooterItemCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }
    public async Task<IResult> Handle(DeleteFooterItemCommand request, CancellationToken cancellationToken)
    {
        var documentType = await _unitOfWork.Repository<MenuItem>().GetByIdAsync(request.Id);
            if (documentType != null)
            {
                await _unitOfWork.Repository<MenuItem>().DeleteAsync(documentType);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFooterItem);
                return await Result<int>.SuccessAsync(documentType.Id, _localizer["Footer Item Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Footer Item Not Found!"]);
            }
    }
}