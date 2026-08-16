using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.FestivalLikes;

public class GetUserLikeStateQuery:IRequest<IResult<bool>>
{
    public int? FestivalId { get; set; }
    public int? NewId { get; set; }
}

public class GetUserLikeFestivalStateQueryHandler
    (IUnitOfWork<int> unitOfWork,ICurrentUserService currentUserService):IRequestHandler<GetUserLikeStateQuery, IResult<bool>>
{
    public async Task<IResult<bool>> Handle(GetUserLikeStateQuery request, CancellationToken cancellationToken)
    {
        var res = await unitOfWork.Repository<Like>()
            .Entities.Where(p => p.FestivalId == request.FestivalId 
                                 &&p.NewId==request.NewId && p.UserId == currentUserService.UserId)
            .AnyAsync(cancellationToken);
        
        return await Result<bool>.SuccessAsync(res);
    }
}
