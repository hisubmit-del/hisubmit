using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Submits.Commands
{
    public class AddEditFinalJudgingCommand:IRequest<IResult>
    {
        public List<int> SubmitId { get; set; }
        public string Comment { get; set; }
        public  JudgingStatus JudgingStatus { get; set; }
        public  SubmitStatus SubmitStatus { get; set; }
    }

    public class AddEditFinalJudgingCommandHandler : IRequestHandler<AddEditFinalJudgingCommand, IResult>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditFinalJudgingCommandHandler> _localizer;
        public AddEditFinalJudgingCommandHandler(IUnitOfWork<int> unitOfWork
            , IStringLocalizer<AddEditFinalJudgingCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<IResult> Handle(AddEditFinalJudgingCommand request, CancellationToken cancellationToken)
        {
            foreach (var sId in request.SubmitId)
            {
                var submit = await _unitOfWork.Repository<Submit>().GetByIdAsync(sId);
                if(submit != null)
                {
                    submit.JudgingStatus = request.JudgingStatus;
                    submit.Comment = request.Comment;
                    submit.SubmitStatus = request.SubmitStatus;
                    await _unitOfWork.Repository<Submit>().UpdateAsync(submit);
                  
                }
                else
                {
                    return await Result.FailAsync(_localizer["Submit not found"]);
                }
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync(_localizer["submit status updated"]);
        }
    }
}
