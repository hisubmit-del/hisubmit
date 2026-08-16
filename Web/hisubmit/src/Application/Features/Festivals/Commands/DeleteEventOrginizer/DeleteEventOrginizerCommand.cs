using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Festivals.Commands.DeleteEventOrginizer
{
    public class DeleteEventOrginizerCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int FestivalId { get; set; }
    }
    public class DeleteEventOrginizerCommandHandler : IRequestHandler<DeleteEventOrginizerCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteEventOrginizerCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        public DeleteEventOrginizerCommandHandler(IStringLocalizer<DeleteEventOrginizerCommandHandler>localizer,
            IUnitOfWork<int>unitOfWork)
        {
            _localizer = localizer;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<int>> Handle(DeleteEventOrginizerCommand request, CancellationToken cancellationToken)
        {
            var orginizer = await _unitOfWork.Repository<EventOrginizer>().GetByIdAsync(request.Id);
            if (orginizer != null)
            {
                await _unitOfWork.Repository<EventOrginizer>().DeleteAsync(orginizer);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllEventOrginizerKey);
                return await Result<int>.SuccessAsync(orginizer.Id, _localizer["Orginizer Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Orginizer Not Found!"]);
            }
        }
    }
}
