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

namespace HiSubmit.Application.Features.Submission.SubmissionQuestions.Commands.Delete
{
    public class DeleteSubmissionQuestionCommand:IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int FestivalId { get; set; }
    }
    public class DeleteSubmissionQuestionCommandHandler : IRequestHandler<DeleteSubmissionQuestionCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteSubmissionQuestionCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        public DeleteSubmissionQuestionCommandHandler(
            IStringLocalizer<DeleteSubmissionQuestionCommandHandler> localizer, IUnitOfWork<int> unitOfWork)
        {
            _localizer = localizer;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(DeleteSubmissionQuestionCommand request, CancellationToken cancellationToken)
        {
            var subQuestion =await _unitOfWork.Repository<SubmissionQuestion>().GetByIdAsync(request.Id);
            if(subQuestion != null)
            {
                await  DeleteCategories(request.Id);
                await _unitOfWork.Repository<SubmissionQuestion>().DeleteAsync(subQuestion);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken,ApplicationConstants.Cache.GetAllsubmissionQuestion);
                return await Result<int>.SuccessAsync(request.Id, _localizer["Question deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Question not found"]);
            }
        }

        private async Task DeleteCategories(int questionId)
        {
            var eventCategoriesSubmission = _unitOfWork.Repository<SubmissionQuestionEventCategory>()
                .Entities.Where(p => p.SubmissionQuestionId == questionId);

            foreach (var item in eventCategoriesSubmission)
            {
               await _unitOfWork.Repository<SubmissionQuestionEventCategory>().DeleteAsync(item);
            }
        }
    }
}
