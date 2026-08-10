using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Filters;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectCreditCommand;

namespace HiSubmit.Application.Features.Projects.Commands.AddEditProjectCreditCommand;

public class UpdateProjectCreditsCommand :UpdateProjectCreditsRequest, IRequest<Result<int>>;
public class AddEditProjectCreditCommandHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    ICheckPermission checkPermission,
    IStringLocalizer<AddEditProjectCreditCommandHandler> localizer)
    : IRequestHandler<UpdateProjectCreditsCommand, Result<int>>
{
    public async Task<Result<int>> Handle(UpdateProjectCreditsCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.Repository<Project>()
            .GetByIdAsync(request.ProjectId);
        if (project == null)
            throw new NullReferenceException();
        await checkPermission.CheckWrightProjectPermission(project.CreatedBy);
            
        var clientIds = request.Credits.Select(p => p.Id).ToList();
        var deletedCredits = unitOfWork.Repository<ProjectCredit>()
            .Entities.Where(p => clientIds.All(id => id != p.Id) && p.ProjectId == request.ProjectId);

        foreach (var credit in request.Credits)
        {
            credit.ProjectId = request.ProjectId;
        }
        foreach (var credit in deletedCredits)
        {
            await unitOfWork.Repository<ProjectCredit>().DeleteAsync(credit);
        }
        foreach (var creditRequest in request.Credits)
        {
            if (creditRequest.Id == 0)
            {
                var credit = mapper.Map<ProjectCredit>(creditRequest);
                await unitOfWork.Repository<ProjectCredit>().AddAsync(credit);               
            }
            else
            {
                var peopleIds = creditRequest.ProjectItemPeople.Select(p => p.Id);

                var deletedPerson = unitOfWork.Repository<ProjectItemPerson>()
                    .Entities.Where(p => peopleIds.All(id => id != p.Id) && p.ProjectCreditId == creditRequest.Id);
                foreach (var person in deletedPerson)
                {
                    await unitOfWork.Repository<ProjectItemPerson>().DeleteAsync(person);
                }

                var dbCredit = await unitOfWork.Repository<ProjectCredit>().GetByIdAsync(creditRequest.Id);
                if(dbCredit == null)
                {
                    return await Result<int>.FailAsync(localizer["Error accrued in update credit person"]);
                }
                var updatedCredit = mapper.Map(creditRequest, dbCredit);
                await unitOfWork.Repository<ProjectCredit>().UpdateAsync(updatedCredit);      
            }
        }
        await unitOfWork.CommitAndRemoveCache(cancellationToken);
        return await Result<int>.SuccessAsync(0, localizer["credit updated"]);

    }
}