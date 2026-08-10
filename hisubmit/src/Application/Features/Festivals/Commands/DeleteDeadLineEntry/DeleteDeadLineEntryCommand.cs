using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Festivals.Commands.DeleteDeadLineEntry
{
    public class DeleteDeadLineEntryCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int FestivalId { get; set; }
    }
    public class DeleteDeadLineEntryCommandHandler : IRequestHandler<DeleteDeadLineEntryCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteDeadLineEntryCommandHandler> _localizer;
        public DeleteDeadLineEntryCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteDeadLineEntryCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteDeadLineEntryCommand request, CancellationToken cancellationToken)
        {
            var deadLine = await _unitOfWork.Repository<DeadLine>().GetByIdAsync(request.Id);
            if (deadLine != null)
            {
                await DeleteCategory(deadLine.Id);
                await _unitOfWork.Repository<DeadLine>().DeleteAsync(deadLine);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDeadLineCacheKey);
                return await Result<int>.SuccessAsync(deadLine.Id, _localizer["DeadLine Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["DeadLine Not Found!"]);
            }
        }

        private async Task DeleteCategory(int deadLineId)
        {
            var catsDeadLine = await _unitOfWork.Repository<DeadlineEventCategory>()
                .Entities.Where(p => p.DeadLineId == deadLineId)
                .ToListAsync();

            if (catsDeadLine != null)
            {
                foreach (var item in catsDeadLine)
                {
                    await _unitOfWork.Repository<DeadlineEventCategory>()
                         .DeleteAsync(item);
                }
            }
        }
    }
}
