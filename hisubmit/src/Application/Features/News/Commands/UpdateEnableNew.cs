using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Application.Interfaces.Services;

namespace HiSubmit.Application.Features.News.Commands;

public class UpdateEnableNewCommand:IRequest<IResult>
{
    public  int Id { get; set; }
    public bool IsEnable { get; set; }
    public int? FestivalId { get; set; }
}

public class UpdateEnableNewCommandHandler : IRequestHandler<UpdateEnableNewCommand, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<AddEditNewCommandHandler> _localizer;
    private readonly ICurrentUserService _currentUserService;

    public UpdateEnableNewCommandHandler(IUnitOfWork<int> unitOfWork,
        IStringLocalizer<AddEditNewCommandHandler> localizer,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
        _currentUserService = currentUserService;
    }
    public async Task<IResult> Handle(UpdateEnableNewCommand request, CancellationToken cancellationToken)
    {
        var newDb = await _unitOfWork.Repository<New>().GetByIdAsync(request.Id);
        if (newDb == null) return await Result.FailAsync(_localizer["new not found"]);
        if (request.FestivalId.HasValue &&
            newDb.FestivalId != request.FestivalId &&
            !_currentUserService.IsInRole(RoleConstants.AdministratorRole))
            return await Result.FailAsync(_localizer["You cannot publish news from another festival"]);

        newDb.IsEnable = request.IsEnable;
        await _unitOfWork.Repository<New>().UpdateAsync(newDb);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(_localizer["New Updated"]);
    }
}
