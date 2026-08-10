using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.RefeerrAddToProjects;

namespace HiSubmit.Application.Features.ProjectJudgings.Commands;

public record AddEditProjectJudgingCommand
    (List<int> SubmitsId, List<string> UsersId, int FestivalId, bool AssignToReferee) 
    : IRequest<Result<int>>
{
    public bool MultiProjectToMultiReferee { get; set; }
}

public class AddEditProjectJudgingCommandHandler :
    IRequestHandler<AddEditProjectJudgingCommand, Result<int>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<AddEditProjectJudgingCommandHandler> _localizer;
    private readonly IMediator _mediator;

    public AddEditProjectJudgingCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper
        , IStringLocalizer<AddEditProjectJudgingCommandHandler> localizer,IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
        _localizer = localizer;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle
        (AddEditProjectJudgingCommand request, CancellationToken cancellationToken)
    {
        var projectJudgingList = new List<ProjectJudging>();
        if (request.AssignToReferee || request.MultiProjectToMultiReferee)
        {
            foreach (var userId in request.UsersId)
            {
                var refereeId =userId;
                var dbProjectJudgings = _unitOfWork.Repository<ProjectJudging>()
                    .Entities.Where(p => p.UserId == refereeId && p.Submit.FestivalId == request.FestivalId);
            
                foreach (var submitId in request.SubmitsId
                             .Where(submitId => !dbProjectJudgings
                                 .Any(projJud => projJud.SubmitId == submitId)))
                {
                    var projectJudging = new ProjectJudging()
                    {
                        UserId = refereeId,
                        SubmitId = submitId
                    };
                    projectJudgingList.Add(projectJudging);
                    await _unitOfWork.Repository<ProjectJudging>().AddAsync(projectJudging);
                }
                if (!request.MultiProjectToMultiReferee)
                {
                    foreach (var deletedProjJudging in dbProjectJudgings
                            .Where(proJudg => request.SubmitsId.All(submitId => submitId != proJudg.SubmitId)))
                    {
                        await _unitOfWork.Repository<ProjectJudging>().DeleteAsync(deletedProjJudging);
                    }
                }
            }
        }
        else
        {
            var submitId = request.SubmitsId[0];
            var dbProjectJudgings = _unitOfWork.Repository<ProjectJudging>()
                .Entities.Where(p => p.SubmitId == submitId && 
                                     p.Submit.FestivalId == request.FestivalId);

            foreach (var userId in request.UsersId
                         .Where(userId => !dbProjectJudgings
                             .Any(projJud => projJud.UserId == userId)))
            {
                var projectJudging = new ProjectJudging()
                {
                    UserId = userId,
                    SubmitId = submitId,
                };
                projectJudgingList.Add(projectJudging);
                await _unitOfWork.Repository<ProjectJudging>().AddAsync(projectJudging);
            }
            foreach (var deletedProjJudging in dbProjectJudgings
                         .Where(p => request.UsersId
                             .All(userId => userId != p.UserId)))
            {
                await _unitOfWork.Repository<ProjectJudging>().DeleteAsync(deletedProjJudging);
            }
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (var projectJudging in projectJudgingList)
        {
            await _mediator.Publish(new RefereeAddToProjectsEvent()
            {
                UserId = projectJudging.UserId,
                ProjectJudgingId = projectJudging.Id,
            },cancellationToken);
        }
        return await Result<int>.SuccessAsync("Judging  updated");
    }
}