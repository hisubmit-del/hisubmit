using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Requests.AnswerQuestions;

namespace HiSubmit.Application.Features.Submits.Queries.GetSubmitFormAnswers;

public class GetSubmitFormAnswersQuery:IRequest<IResult<List<AnswerQuestionDto>>>
{
    public  int SubmitId { get; set; }
    public int FestivalId { get; set; }
}

public class GetSubmitFormAnswersQueryHandler :
    IRequestHandler<GetSubmitFormAnswersQuery, IResult<List<AnswerQuestionDto>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetSubmitFormAnswersQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<List<AnswerQuestionDto>>> Handle
        (GetSubmitFormAnswersQuery request, CancellationToken cancellationToken)
    {
        var answers = await _unitOfWork.Repository<SubmitAnswerQuestion>()
            .Entities
            .Where(p => p.SubmitId == request.SubmitId)
            .ProjectTo<AnswerQuestionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return await Result<List<AnswerQuestionDto>>.SuccessAsync(answers);
    }
}