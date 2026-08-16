using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Festivals.Commands.DeleteEventCategory
{
    public class DeleteEventCategoryCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
    }
    internal class DeleteEventCategoryCommandHandler : IRequestHandler<DeleteEventCategoryCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteEventCategoryCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        public DeleteEventCategoryCommandHandler(IStringLocalizer<DeleteEventCategoryCommandHandler> localizer,
            IUnitOfWork<int> unitOfWork)
        {
            _localizer = localizer;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(DeleteEventCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Repository<EventCategory>().GetByIdAsync(request.Id);
            if (category != null)
            {
                await DeleteDeadLineCategory(request.Id);
                await _unitOfWork.Repository<EventCategory>().DeleteAsync(category);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllEventCategoryCacheKefy);
                return await Result<int>.SuccessAsync(category.Id, _localizer["Category Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["category Not Found!"]);
            }
        }

        private async Task DeleteDeadLineCategory(int catId)
        {
            var deadLineCats = _unitOfWork.Repository<DeadlineEventCategory>().Entities
                .Where(p => p.EventCategoryId == catId);
            foreach (var item in deadLineCats)
            {
               await _unitOfWork.Repository<DeadlineEventCategory>().DeleteAsync(item);
            }
        }
    }
}
