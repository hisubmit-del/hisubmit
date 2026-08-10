using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals;
using Hisubmit.Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetRefereeData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.ProjectJudgings.Queries.GetRefereeData;

public class GetRefereeDataQuery : GetRefereeDataRequest, IRequest<IResult<GetRefereeDataResponse>>;

public class GetRefereeDataQueryHandler(IUnitOfWork<int> unitOfWork, ICurrentUserService currentUserService)
    : IRequestHandler<GetRefereeDataQuery, IResult<GetRefereeDataResponse>>
{
    public async Task<IResult<GetRefereeDataResponse>> Handle(GetRefereeDataQuery request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        if (request.GetCurrentUserData)
            userId = currentUserService.UserId;

        var ratedReferee = await unitOfWork.Repository<ProjectJudging>()
            .Entities
            .Where(p => p.UserId == userId && p.JudgingFiledAnswereds.Any())
            .CountAsync(cancellationToken);

        var notRatedReferee = await unitOfWork.Repository<ProjectJudging>()
            .Entities
            .Where(p => p.UserId == userId && !p.JudgingFiledAnswereds.Any())
            .CountAsync(cancellationToken);
        double averageRated = 0;
        if (await unitOfWork.Repository<JudgingFiledAnswered>()
                .Entities
                .AnyAsync(p => p.ProjectJudging.UserId == userId, cancellationToken: cancellationToken))
        {
         averageRated = await unitOfWork.Repository<JudgingFiledAnswered>()
            .Entities
            .Where(p => p.ProjectJudging.UserId == userId)
            .AverageAsync(p => p.Rate,cancellationToken);
        }

        return await Result<GetRefereeDataResponse>.SuccessAsync(new GetRefereeDataResponse()
        {
            AverageRate = averageRated,
            RatedProject = ratedReferee,
            NotRatedProject = notRatedReferee
        });
    }
}