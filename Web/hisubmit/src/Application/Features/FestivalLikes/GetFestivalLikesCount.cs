using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Likes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.FestivalLikes;

public class GetLikesCountQuery :GetLikeCountRequest, IRequest<IResult<int>>;


public class GetFestivalLikesCountQueryHandler
    (IUnitOfWork<int> unitOfWork): IRequestHandler<GetLikesCountQuery, IResult<int>>
{
    public async Task<IResult<int>> Handle(GetLikesCountQuery request, CancellationToken cancellationToken)
    {
        var count=await unitOfWork.Repository<Like>()
            .Entities
            .CountAsync(p=>p.FestivalId==request.FestivalId && p.NewId==request.NewId, cancellationToken: cancellationToken);   
        return await Result<int>.SuccessAsync(count);
    }
}

