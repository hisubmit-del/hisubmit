using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
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

namespace HiSubmit.Application.Features.Submission.SubmissionQuestions.Query.GetAll
{
    public class GetAllSubmissionQuestionQuery : IRequest<Result<List<GetAllSubmissionQuestionResponse>>>
    {
        public int? FestivalId { get; set; }
        public string CategoriesIdString { get; set; }
        public int? JudgingId { get; set; }
        public bool IncludeAnswer { get; set; }

        public GetAllSubmissionQuestionQuery()
        {
           
        }

        public List<int> GetRealCategories()
        {
            return CategoriesIdString.Split(',').Select(p => int.Parse(p)).ToList();
        }

    }
    public class GetAllSubmissionQuestionQueryHandler :
        IRequestHandler<GetAllSubmissionQuestionQuery, Result<List<GetAllSubmissionQuestionResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<GetAllSubmissionQuestionQueryHandler> _localizer;
        public GetAllSubmissionQuestionQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork,
            IStringLocalizer<GetAllSubmissionQuestionQueryHandler> localizer)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<List<GetAllSubmissionQuestionResponse>>> Handle(GetAllSubmissionQuestionQuery request, CancellationToken cancellationToken)
        {
            IQueryable<SubmissionQuestion> query=_unitOfWork.Repository<SubmissionQuestion>().Entities;
            if (request.JudgingId != null)
            {
                query = query.Where(p => p.JudgingId == request.JudgingId);
            }
            else
            {
                query = query.Where(p => p.FestivalId == request.FestivalId &&p.JudgingId==null);
            }
             // command = _unitOfWork.Repository<SubmissionQuestion>().Entities
             //    .Where(p => p.ProductFestivalId == request.ProductFestivalId && p.JudgingId == request.JudgingId);
             //
            if (request.IncludeAnswer)
            {
                query = query.Include(p => p.Options);
            }
            if (request.CategoriesIdString != null && request.CategoriesIdString.Any())
            {
                query = query.Include(p => p.SubmissionQuestionEventCategories);
                query = query.Where(p => p.ApplyforAllCategory ||
                                         p.SubmissionQuestionEventCategories
                                             .Any(cat => request.GetRealCategories()
                                                 .Any(id => id == cat.Id)));
            }

            var GetAll = await query
                .ToListAsync();

            var mappedQuestions = _mapper.Map<List<GetAllSubmissionQuestionResponse>>(GetAll);
            return await Result<List<GetAllSubmissionQuestionResponse>>.SuccessAsync(mappedQuestions);
        }
    }
}
