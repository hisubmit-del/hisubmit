using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.FestivalLikes;

public class AddOrDeleteLikeCommand:IRequest<IResult>
{
    public int? FestivalId { get; set; }
    public int? NewId { get; set; }
}

public class AddOrDeleteFestivalLikeCommandHandler(IUnitOfWork<int> unitOfWork,ICurrentUserService currentUserService)
    :IRequestHandler<AddOrDeleteLikeCommand,IResult>
{
    public async Task<IResult> Handle(AddOrDeleteLikeCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            return await Result.FailAsync("You must be logged in to like this content");

        if ((request.FestivalId.HasValue && request.NewId.HasValue) ||
            (!request.FestivalId.HasValue && !request.NewId.HasValue))
            return await Result.FailAsync("A festival or news item is required");

        var userId=currentUserService.UserId;

        var like=await unitOfWork.Repository<Like>()
            .Entities
            .Where(p=>p.FestivalId==request.FestivalId && p.UserId == userId && request.NewId==p.NewId)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (like == null)
            await unitOfWork.Repository<Like>()
                .AddAsync(new Like()
                {
                    UserId = userId,
                    FestivalId = request.FestivalId,
                    NewId=request.NewId
                });
        else
            await unitOfWork.Repository<Like>()
                .DeleteAsync(like);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}
