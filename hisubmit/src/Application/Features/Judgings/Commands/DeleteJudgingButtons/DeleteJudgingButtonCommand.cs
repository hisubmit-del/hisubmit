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

namespace HiSubmit.Application.Features.Judgings.Commands.DeleteJudgingButtons
{
    public class DeleteJudgingButtonCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
    }
    internal class DeleteJudgingButtonCommandHandler : IRequestHandler<DeleteJudgingButtonCommand, Result<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteJudgingButtonCommandHandler> _localizer;
        public DeleteJudgingButtonCommandHandler(IUnitOfWork<int> unitOfWork,
            IStringLocalizer<DeleteJudgingButtonCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }
        public async Task<Result<int>> Handle(DeleteJudgingButtonCommand request, CancellationToken cancellationToken)
        {
            var button =await _unitOfWork.Repository<JudgingButton>().GetByIdAsync(request.Id);
            if(button != null)
            {
                await _unitOfWork.Repository<JudgingButton>().DeleteAsync(button);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(button.Id, _localizer["button deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["button not found"]);
            }
        }
    }
}
