using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Features.ProjectJudgings.Commands.AddProjectJudgingResult;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests.AnswerQuestions;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HiSubmit.Client.SharedModels.Constants.Role;

namespace HiSubmit.Application.Features.ProjectJudgings.Queries.GetDetail;

public class GetProjectJudgingDetailQuery:IRequest<Result<GetProjectJudgingDetailResponse>>
{
    public int Id { get; set; }
    public  int SubmitId { get; set; }
    public bool GetUserReferee { get; set; }
}

public class  GetProjectJudgingDetailQueryHandler:
    IRequestHandler<GetProjectJudgingDetailQuery,Result<GetProjectJudgingDetailResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public GetProjectJudgingDetailQueryHandler
        (IMapper mapper, IUnitOfWork<int> unitOfWork,
            IUserService userService,ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userService = userService;
        _currentUserService = currentUserService;
    }
    public async Task<Result<GetProjectJudgingDetailResponse>> Handle(GetProjectJudgingDetailQuery request, CancellationToken cancellationToken)
    {
        // if (request.GetUserReferee)
        // {
        //     var userId = _currentUserService.UserId;
        //     request.Id=await _unitOfWork.Repository<Judging>()
        // }
        var dbProjectJudging = await _unitOfWork.Repository<ProjectJudging>()
            .Entities
            .Where(judging => judging.Id == request.Id)
            .Select(judging => new { judging.Id, judging.UserId, judging.RefereeStatus })
            .FirstOrDefaultAsync(cancellationToken);

        if (dbProjectJudging == null)
            return await Result<GetProjectJudgingDetailResponse>.FailAsync("Judging assignment not found");

        if (!_currentUserService.IsAuthenticated ||
            (!_currentUserService.IsInRole(RoleConstants.AdministratorRole) &&
             (dbProjectJudging.UserId != _currentUserService.UserId ||
              dbProjectJudging.RefereeStatus != RefereeStatus.Default)))
            return await Result<GetProjectJudgingDetailResponse>.FailAsync("You do not have access to this judging assignment");

        var projectJudging = await _unitOfWork.Repository<ProjectJudging>()
            .Entities
            .Where(judging => judging.Id == dbProjectJudging.Id)
            .Include(p => p.JudgingButton)
            .Include(p => p.JudgingFiledAnswereds)
            .Include(p => p.SubmitAnswerQuestions)
            .Include(p => p.Submit).ThenInclude(p => p.Project)
            .ProjectTo<GetProjectJudgingDetailResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (projectJudging != null)
        {
            var userName = (await _userService
                .GetUserName(new List<string>() {projectJudging.UserId}))[projectJudging.UserId];
            
            projectJudging.UserName = userName;
        }
        return await Result<GetProjectJudgingDetailResponse>.SuccessAsync(projectJudging);
    }
}

public class GetProjectJudgingDetailResponse
{
    public SubmitDto Submit { get; set; }
    public int SubmitId { get; set; }
    public string UserId { get; set; }    
    public int? JudgingButtonId { get; set; }
    public JudgingButton JudgingButton { get; set; }
    public  string Comment { get; set; }

    public  string UserName { get; set; }
    public List<JudgingFieldAnswerDto> JudgingFiledAnswereds { get; set; }
    public List<AnswerQuestionDto> SubmitAnswerQuestions { get; set; }

    public GetProjectJudgingDetailResponse()
    {
        JudgingFiledAnswereds = new List<JudgingFieldAnswerDto>();
        SubmitAnswerQuestions = new List<AnswerQuestionDto>();
    }
}


public class SubmitDto
{
    public  int ProjectId { get; set; }
    public  string ProjectTitle { get; set; }
    public  ProjectType ProjectProjectType { get; set; }
    public  int FestivalId { get; set; }
    public DateTime SubmitDate { get; set; }
    public SubmitStatus SubmitStatus { get; set; }
    public string Comment { get; set; }
    
   public string TrackingCode { get; set; }
}
