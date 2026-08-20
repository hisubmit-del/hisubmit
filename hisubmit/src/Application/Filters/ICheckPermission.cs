using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSubmit.Application.Exceptions;
using HiSubmit.Application.Features.Projects.Queries.GetDetail;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Constants.Role;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Filters;

public interface ICheckPermission
{
    Task<bool> CheckReadProjectPermission(int projectId);
    Task<bool> CheckReadProjectPermission(int projectId,string projectCreatorId);
    Task<bool> CheckReadProjectPermission(Project project);
    Task<bool> CheckWrightProjectPermission(string projectCreatorId);
}

public class CheckPermission : ICheckPermission
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CheckPermission(IUnitOfWork<int> unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> CheckReadProjectPermission(int projectId)
    {
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(projectId);
        return await CheckReadProjectPermission(project);
    }

    public async Task<bool> CheckReadProjectPermission(int projectId, string projectCreatorId)
    {
        if (projectCreatorId == _currentUserService.UserId ||
            _currentUserService.IsInRole(RoleConstants.AdministratorRole))
            return true;

        
        var projectSubmits = await _unitOfWork.Repository<Submit>()
            .Entities.Where(p => p.ProjectId == projectId)
            .Select(p => new
            {
                FestivalId = p.FestivalId,
                JudgmentIds = p.ProjectJudgings
                    .Where(judging =>
                        judging.RefereeStatus == RefereeStatus.Default &&
                        p.Festival.FestivalSubUsers.Any(member =>
                            member.UserId == _currentUserService.UserId &&
                            member.IsReferee &&
                            !member.IsRemoved))
                    .Select(judging => judging.UserId)
            })
            .ToListAsync();

        var allowedFestivalIds = await _unitOfWork.Repository<Festival>()
            .Entities
            .Where(festival => festival.UserId == _currentUserService.UserId ||
                               (festival.IsActive &&
                                festival.FestivalSubUsers.Any(member =>
                                   member.UserId == _currentUserService.UserId &&
                                   !member.IsReferee &&
                                   !member.IsRemoved)))
            .Select(festival => festival.Id)
            .ToListAsync();

        if (projectSubmits.Any(p => allowedFestivalIds.Contains(p.FestivalId)))
            return true;

        
        //check project assign to current user for judging
        var jId = new List<string>();
        foreach (var pSubmit in projectSubmits)
        {
            jId.AddRange(pSubmit.JudgmentIds);
        }

        return jId.Any(p => p == _currentUserService.UserId);
    }

    public async Task<bool> CheckReadProjectPermission(Project project)
    {
        return await CheckReadProjectPermission(project.Id, project.UserId);
    }

    public async Task<bool> CheckWrightProjectPermission(string projectCreatorId)
    {
        if (projectCreatorId == _currentUserService.UserId)
            return true;
        throw new DontPermissionException();
    }
}

