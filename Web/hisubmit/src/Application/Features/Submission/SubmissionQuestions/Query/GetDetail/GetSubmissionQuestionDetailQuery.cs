using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Submission.SubmissionQuestions.Query.GetDetail
{
    public class GetSubmissionQuestionDetailQuery:IRequest<Result<GetSubmissionQuestionDetailResponse>>
    {
        public int FestivalId { get; set; }
        public int  Id { get; set; }
    }
    public class GetSubmissionQuestionDetailQueryHandler : IRequestHandler<GetSubmissionQuestionDetailQuery, Result<GetSubmissionQuestionDetailResponse>>
    {
        private readonly IStringLocalizer<GetSubmissionQuestionDetailQueryHandler> _Localizer;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        public GetSubmissionQuestionDetailQueryHandler(
            IStringLocalizer<GetSubmissionQuestionDetailQueryHandler> localizer, 
            IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _Localizer = localizer;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetSubmissionQuestionDetailResponse>> Handle(GetSubmissionQuestionDetailQuery request, CancellationToken cancellationToken)
        {
            var subQuestion = await _unitOfWork.Repository<SubmissionQuestion>()
                .Entities.Where(p => p.Id == request.Id)
                .Include(p => p.SubmissionQuestionEventCategories)
                .Include(p => p.Options)
                .ProjectTo<GetSubmissionQuestionDetailResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if(subQuestion != null)
            {
                return await Result<GetSubmissionQuestionDetailResponse>.SuccessAsync(subQuestion);
            }
            else
            {
                return await Result<GetSubmissionQuestionDetailResponse>.FailAsync(_Localizer["question not found"]);
            }
        }
    }
}
