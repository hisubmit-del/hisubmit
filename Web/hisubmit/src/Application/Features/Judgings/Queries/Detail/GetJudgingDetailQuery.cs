using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Features.Submission.SubmissionQuestions.Query.GetAll;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Judgings.Queries.Detail
{
    public class GetJudgingDetailQuery:IRequest<Result<GetJudgingDetailResponse>>
    {
        public int FestivalId { get; set; }
        public  ProjectType ProjectType { get; set; }
    }

    public class GetJudgingDetailQueryHandler : IRequestHandler<GetJudgingDetailQuery, Result<GetJudgingDetailResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetJudgingDetailQueryHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetJudgingDetailQueryHandler(
            IMapper mapper, IStringLocalizer<GetJudgingDetailQueryHandler> localizer
            , IUnitOfWork<int> unitOfWork)
        {
            _mapper = mapper;
            _localizer = localizer;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GetJudgingDetailResponse>> Handle(GetJudgingDetailQuery request, CancellationToken cancellationToken)
        {
            var judging =await _unitOfWork.Repository<Judging>().Entities
                .Where(p=>p.FestivalId==request.FestivalId && p.ProjectType==request.ProjectType)
                .Include(p => p.JudgingButtons)
                .Include(p => p.JudgingFileds)
                .Include(p => p.SubmissionQuestions)
                .ProjectTo<GetJudgingDetailResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if(judging != null)
            {
                return await Result<GetJudgingDetailResponse>.SuccessAsync(judging);
            }
            else
            {
                return await Result<GetJudgingDetailResponse>.FailAsync(_localizer["Judgiing not found"]);
            }
        }
    }
}
