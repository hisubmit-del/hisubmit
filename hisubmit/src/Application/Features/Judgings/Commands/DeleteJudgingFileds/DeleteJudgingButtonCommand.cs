using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Judgings.Commands.DeleteJudgiingFiiled
{
    public class DeleteJudgingFiledCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
    }
    internal class DeleteJudgingFiledCommandHandler : IRequestHandler<DeleteJudgingFiledCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteJudgingFiledCommandHandler> _localizer;
        public DeleteJudgingFiledCommandHandler(IUnitOfWork<int> unitOfWork,
            IStringLocalizer<DeleteJudgingFiledCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }
        public async Task<Result<int>> Handle(DeleteJudgingFiledCommand request, CancellationToken cancellationToken)
        {
            var filed =await _unitOfWork.Repository<JudgingFiled>().GetByIdAsync(request.Id);
            if(filed != null)
            {
                await _unitOfWork.Repository<JudgingFiled>().DeleteAsync(filed);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(filed.Id, _localizer["filed deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["filed not found"]);
            }
        }
    }
}
