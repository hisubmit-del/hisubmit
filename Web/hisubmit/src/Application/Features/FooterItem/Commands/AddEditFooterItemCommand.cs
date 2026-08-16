using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Domain.Entities.Misc;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.FooterItems.Commands;

public class AddEditFooterItemCommand:FooterItemDto,IRequest<IResult>
{
    
}

public class AddEditFooterItemCommandHandler : IRequestHandler<AddEditFooterItemCommand,IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<AddEditFooterItemCommandHandler> _localizer;
    private readonly IMapper _mapper;

    public AddEditFooterItemCommandHandler
        (IUnitOfWork<int> unitOfWork, IStringLocalizer<AddEditFooterItemCommandHandler> localizer, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
        _mapper = mapper;
    }
    public async Task<IResult> Handle(AddEditFooterItemCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == 0)
        {
            var footerItem = _mapper.Map<MenuItem>(command);
            await _unitOfWork.Repository<MenuItem>().AddAsync(footerItem);
            await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFooterItem);
            return await Result<int>.SuccessAsync(footerItem.Id, _localizer["Footer Item Saved"]);
        }
        else
        {
            var footerItemDb = await _unitOfWork.Repository<MenuItem>().GetByIdAsync(command.Id);
            if (footerItemDb != null)
            {
                var updatedFooterItem = _mapper.Map(command, footerItemDb);
                await _unitOfWork.Repository<MenuItem>().UpdateAsync(updatedFooterItem);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFooterItem);
                return await Result<int>.SuccessAsync(footerItemDb.Id, _localizer["Footer Item Updated"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Footer Item Not Found!"]);
            }
        }
    }
}