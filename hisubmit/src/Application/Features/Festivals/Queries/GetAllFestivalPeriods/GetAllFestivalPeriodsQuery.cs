using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllFestivalPeriods;

public class GetAllFestivalPeriodsQuery : IRequest<IResult<GetAllFestivalPeriodsResponse>>
{
    public int FestivalId { get; set; }
    public int FestivalMasterId { get; set; }
}

public class GetAllFestivalPeriodsQueryHandler : IRequestHandler<GetAllFestivalPeriodsQuery,IResult<GetAllFestivalPeriodsResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<GetAllFestivalPeriodsQuery> _localize;

    public GetAllFestivalPeriodsQueryHandler
        (IUnitOfWork<int> unitOfWork, IStringLocalizer<GetAllFestivalPeriodsQuery> localize)
    {
        _unitOfWork = unitOfWork;
        _localize = localize;
    }

    public async Task<IResult<GetAllFestivalPeriodsResponse>> Handle(GetAllFestivalPeriodsQuery request, CancellationToken cancellationToken)
    {
        var festivalMasterId = request.FestivalMasterId;
        if (request.FestivalId != 0)
        {
            festivalMasterId = await _unitOfWork.Repository<Festival>()
                .Entities.Where(p => p.Id == request.FestivalId)
                .Select(p => p.FestivalMasterId)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        var festivalPeriods = await _unitOfWork.Repository<Festival>()
            .Entities.Where(p => p.FestivalMasterId == festivalMasterId)
            .Select(p => new FestivalPeriod
            {
                FestivalId = p.Id,
                Period = p.YearsRunning
            })
            .ToListAsync(cancellationToken);

        var result = new GetAllFestivalPeriodsResponse
        {
            FestivalPeriods = festivalPeriods,
            FestivalMasterId = festivalMasterId
        };

        return await Result<GetAllFestivalPeriodsResponse>.SuccessAsync(result);
    }
}

public class GetAllFestivalPeriodsResponse
{
    public int FestivalMasterId { get; set; }
    public List<FestivalPeriod> FestivalPeriods { get; set; }

    public GetAllFestivalPeriodsResponse()
    {
        FestivalPeriods = new List<FestivalPeriod>();
    }
}

public class FestivalPeriod
{
    public int FestivalId { get; set; }
    public int Period { get; set; }
}