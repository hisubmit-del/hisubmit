using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;
using Hisubmit.Hisubmit.Client.SharedModels.Features.AdminDashboard;

namespace HiSubmit.Application.Features.AdminDashboard.Queries;

public class GetFestivalAndUserStatusCountQuery 
    : IRequest<IResult<GetFestivalAndUserStatusCount>>;

public class GetFestivalAndUserStatusCountQueryHandler(IUnitOfWork<int> unitOfWork,IUserService userService)
    :IRequestHandler<GetFestivalAndUserStatusCountQuery,IResult<GetFestivalAndUserStatusCount>>
{
    public async Task<IResult<GetFestivalAndUserStatusCount>>
        Handle(GetFestivalAndUserStatusCountQuery request, CancellationToken cancellationToken)
    {
        var model = new GetFestivalAndUserStatusCount();

        var festivalMasterQuery =  unitOfWork.Repository<FestivalMaster>()
            .Entities;

        model.FestivalCount =await festivalMasterQuery.CountAsync(cancellationToken: cancellationToken);

        model.ActiveFestivalCount = await unitOfWork.Repository<Festival>()
            .Entities.Where(p => p.NotificationDate <= DateTime.Today && p.EventEndDate >= DateTime.Today)
            .CountAsync(cancellationToken);

        model.AllAccountCount = await userService.GetCountAsync();

        model.ProjectsCount = await unitOfWork.Repository<Project>()
            .Entities.CountAsync(cancellationToken);
        model.SubmitCount = await unitOfWork.Repository<Submit>().Entities.CountAsync(cancellationToken);

        return await Result<GetFestivalAndUserStatusCount>.SuccessAsync(model);
    }
}


