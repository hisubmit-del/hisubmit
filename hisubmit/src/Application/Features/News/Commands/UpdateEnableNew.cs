using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.News.Commands;

public class UpdateEnableNewCommand:IRequest<IResult>
{
    public  int Id { get; set; }
    public bool IsEnable { get; set; }
}

public class UpdateEnableNewCommandHandler : IRequestHandler<UpdateEnableNewCommand, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<AddEditNewCommandHandler> _localizer;

    public UpdateEnableNewCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<AddEditNewCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }
    public async Task<IResult> Handle(UpdateEnableNewCommand request, CancellationToken cancellationToken)
    {
        var newDb = await _unitOfWork.Repository<New>().GetByIdAsync(request.Id);
        if (newDb == null) return await Result.FailAsync(_localizer["new not found"]);
        newDb.IsEnable = request.IsEnable;
        await _unitOfWork.Repository<New>().UpdateAsync(newDb);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(_localizer["New Updated"]);
    }
}