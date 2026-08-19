using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Application.Filters;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Exceptions;
using HiSubmit.Domain.Entities.Projects;
using Microsoft.Extensions.Localization;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Identity;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Client.SharedModels.Constants.Role;

using System;

namespace HiSubmit.Application.Features.Projects.Queries.GetDetail;

public class GetProjectDetailQuery:IRequest<Result<GetProjectDetailResponse>>
{
    public int Id { get; set; }
    public string URL { get; set; }
}
public class GetProjectDetailQueryHandler(
    IMapper mapper,
    IUserService userService,
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<GetProjectDetailQueryHandler> localizer,
    ICheckPermission checkPermission,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetProjectDetailQuery, Result<GetProjectDetailResponse>>
{
    private readonly IStringLocalizer<GetProjectDetailQueryHandler> _localizer = localizer;

    public async Task<Result<GetProjectDetailResponse>> Handle(GetProjectDetailQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Project> query;
        if (!string.IsNullOrWhiteSpace(request.URL))
        {
            query = unitOfWork.Repository<Project>().Entities.Where(p => p.URL == request.URL);
        }
        else
        {
            query = unitOfWork.Repository<Project>().Entities.Where(p => p.Id == request.Id);
        }

        var project =await query
            .ProjectTo<GetProjectDetailResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (project is null)
            return await Result<GetProjectDetailResponse>.FailAsync("Project not found");

        var userAccount = await userService.GetAsync(project.UserId);

        project.UserImageUrl = userAccount.Data?.ProfilePictureDataUrl;
        project.UserFullName = userAccount.Data?.FullName;
        if(! await checkPermission.CheckReadProjectPermission(project.Id,project.UserId))
            throw new DontPermissionException();

        var currentUserId = currentUserService.UserId;
        var isAuthenticated = currentUserService.IsAuthenticated &&
                              !string.IsNullOrWhiteSpace(currentUserId);
        var isOwner = isAuthenticated &&
                      string.Equals(currentUserId, project.UserId, StringComparison.Ordinal);
        var isAdministrator = currentUserService.IsInRole(RoleConstants.AdministratorRole);

        var registrations = await unitOfWork.Repository<Submit>()
            .Entities
            .Where(submit => submit.ProjectId == project.Id)
            .Select(submit => new
            {
                submit.Id,
                submit.FestivalId,
                FestivalName = submit.Festival.Name,
                submit.SubmitDate,
                submit.SubmitStatus,
                submit.JudgingStatus,
                submit.TrackingCode
            })
            .ToListAsync(cancellationToken);

        var festivalIds = await unitOfWork.Repository<Festival>()
            .Entities
            .Where(festival => festival.UserId == currentUserId ||
                               festival.FestivalSubUsers.Any(member =>
                                   member.UserId == currentUserId && !member.IsRemoved))
            .Select(festival => festival.Id)
            .ToListAsync(cancellationToken);

        var assignedSubmitIds = await unitOfWork.Repository<ProjectJudging>()
            .Entities
            .Where(judging => judging.Submit.ProjectId == project.Id &&
                              judging.UserId == currentUserId &&
                              judging.RefereeStatus == Domain.Enums.RefereeStatus.Default)
            .Select(judging => judging.SubmitId)
            .ToListAsync(cancellationToken);

        var visibleRegistrations = isOwner || isAdministrator
            ? registrations
            : registrations.Where(item =>
                festivalIds.Contains(item.FestivalId) ||
                assignedSubmitIds.Contains(item.Id));

        if (isOwner || isAdministrator || visibleRegistrations.Any())
        {
            project.CanViewFestivalRegistrations = true;
            project.FestivalRegistrations = visibleRegistrations.Select(item => new ProjectFestivalRegistrationResponse
            {
                SubmitId = item.Id,
                FestivalId = item.FestivalId,
                FestivalName = item.FestivalName,
                SubmitDate = item.SubmitDate,
                SubmitStatus = (Hisubmit.Client.SharedModels.Enums.SubmitStatus)item.SubmitStatus,
                JudgingStatus = (Hisubmit.Client.SharedModels.Enums.JudgingStatus)item.JudgingStatus,
                TrackingCode = item.TrackingCode
            }).ToList();
        }

        var judgingQuery = unitOfWork.Repository<ProjectJudging>()
            .Entities
            .Where(judging => judging.Submit.ProjectId == project.Id &&
                              judging.RefereeStatus == Domain.Enums.RefereeStatus.Default &&
                              (isAdministrator ||
                               judging.UserId == currentUserId ||
                               festivalIds.Contains(judging.Submit.FestivalId)));

        project.CanViewJudgingDetails = isAuthenticated &&
                                        await judgingQuery.AnyAsync(cancellationToken);

        if (project.CanViewJudgingDetails)
        {
            project.JudgingAssignments = await judgingQuery
                .Select(judging => new ProjectJudgingSummaryResponse
                {
                    SubmitId = judging.SubmitId,
                    FestivalId = judging.Submit.FestivalId,
                    FestivalName = judging.Submit.Festival.Name,
                    RefereeUserId = judging.UserId,
                    RefereeStatus = (Hisubmit.Client.SharedModels.Enums.RefereeStatus)judging.RefereeStatus,
                    JudgingButtonId = judging.JudgingButtonId
                })
                .ToListAsync(cancellationToken);
        }

        return await Result<GetProjectDetailResponse>.SuccessAsync(project);
    }
}
