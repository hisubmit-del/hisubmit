using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using HiSubmit.Application.Exceptions;
using Microsoft.Extensions.Localization;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Requests.AnswerQuestions;
using HiSubmit.Application.Events.RefereeSubmitJudgingForProject;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.ProjectJudgings.Commands.AddProjectJudgingResult;

public class AddEditProjectJudgingResultCommand : IRequest<Result<int>>
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public int? JudgingButtonId { get; set; }

    public List<AnswerQuestionDto> SubmitAnswerQuestions { get; set; }
    public List<JudgingFieldAnswerDto> JudgingFiledAnswers { get; set; }
}

public class AddEditProjectJudgingResultCommandHandler 
    : IRequestHandler<AddEditProjectJudgingResultCommand, Result<int>>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<AddEditProjectJudgingResultCommandHandler> _localize;

    public AddEditProjectJudgingResultCommandHandler(ICurrentUserService currentUserService
        , IStringLocalizer<AddEditProjectJudgingResultCommandHandler> localize, IUnitOfWork<int> unitOfWork,
        IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _localize = localize;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(AddEditProjectJudgingResultCommand request,
        CancellationToken cancellationToken)
    {
        
        var dbProjectJudging = await _unitOfWork.Repository<ProjectJudging>()
            .Entities.Include(p => p.JudgingFiledAnswereds)
            .Include(p=>p.Submit)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (dbProjectJudging == null)
            return await Result<int>.FailAsync(_localize["Judging assignment not found"]);

        if (dbProjectJudging.RefereeStatus != RefereeStatus.Default)
        {
            return await Result<int>
                .FailAsync(_localize["You do not have the necessary access to register a review of this work"]);
        }
            
        if (dbProjectJudging.UserId != _currentUserService.UserId)
        {
            throw new DontPermissionException();
        }


        var updatedProjectJudging = _mapper.Map(request, dbProjectJudging);
        await _unitOfWork.Repository<ProjectJudging>().UpdateAsync(updatedProjectJudging);
        await UpdateFields(request.JudgingFiledAnswers,request.Id,cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(
            new RefereeSubmitJudgingFroProjectEvent
            {
                ProjectJudgingId = updatedProjectJudging.Id,
                SubmitId = updatedProjectJudging.SubmitId,
                FestivalId = updatedProjectJudging.Submit.FestivalId
            },
            cancellationToken);
        return await Result<int>.SuccessAsync(updatedProjectJudging.Id, _localize["judgment submit"]);
    }

    private async Task UpdateFields(List<JudgingFieldAnswerDto> answers,int judgingId,CancellationToken cancellationToken)
    {
        var dbJudgingFields = await _unitOfWork.Repository<JudgingFiledAnswered>()
            .Entities.Where(p => p.ProjectJudgingId == judgingId).ToListAsync(cancellationToken);

        answers ??= new List<JudgingFieldAnswerDto>();
        var deletedAnswers = dbJudgingFields
            .Where(dbAnswer => answers.All(p => p.Id != dbAnswer.Id)).ToList();
            
        var updatedAnswers = answers.Where(p=>p.Id!=0).ToList();
        var addedAnswers = answers.Where(p => p.Id == 0).ToList();
            
        foreach (var deletedAnswer in deletedAnswers)
        {
            await _unitOfWork.Repository<JudgingFiledAnswered>().DeleteAsync(deletedAnswer);
        }

        foreach (var updatedAnswer in from cAnswer
                     in updatedAnswers 
                 let dbAnswer = dbJudgingFields.First(p => p.Id == cAnswer.Id) 
                 select _mapper.Map(cAnswer,dbAnswer))
        {
            await _unitOfWork.Repository<JudgingFiledAnswered>().UpdateAsync(updatedAnswer);
        }
            
        foreach (var addAnswer in addedAnswers
                     .Select(dbAnswer => _mapper.Map<JudgingFiledAnswered>(dbAnswer)))
        {
            addAnswer.ProjectJudgingId = judgingId;
            await _unitOfWork.Repository<JudgingFiledAnswered>().AddAsync(addAnswer);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public class JudgingFieldAnswerDto
{
    public int Id { get; set; }
    public int Rate { get; set; }
    public int JudgingFiledId { get; set; }
}
