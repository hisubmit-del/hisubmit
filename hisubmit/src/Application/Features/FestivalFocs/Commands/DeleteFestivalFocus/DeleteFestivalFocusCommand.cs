using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Catalog;
using Microsoft.Extensions.Localization;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Application.Interfaces.Repositories;

namespace HiSubmit.Application.Features.FestivalFocs.Commands.DeleteFestivalFocus;

public class DeleteFestivalFocusCommand:IRequest<Result<int>>
{
    public int Id { get; set; }
}
public class DeleteFestivalFocusCommandHandler : IRequestHandler<DeleteFestivalFocusCommand, Result<int>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<DeleteFestivalFocusCommandHandler> _localize;
    public DeleteFestivalFocusCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteFestivalFocusCommandHandler> localize)
    {
        _unitOfWork = unitOfWork;
        _localize = localize;
    }
    public async Task<Result<int>> Handle(DeleteFestivalFocusCommand request, CancellationToken cancellationToken)
    {
        var festivalFocus = await _unitOfWork.Repository<FestivalFocus>().GetByIdAsync(request.Id);
        if (festivalFocus != null)
        {
            await _unitOfWork.Repository<FestivalFocus>().DeleteAsync(festivalFocus);
            await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFestivalFocusCacheKey);
            return await Result<int>.SuccessAsync(festivalFocus.Id, _localize["festivalFocus Deleted"]);
        }
        else
        {
            return await Result<int>.FailAsync(_localize["festivalFocus Not Found!"]);
        }
    }
}