using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Features.Submits.Queries.GetAllSubmitsQueries;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Submits.Queries.GetSubmitDetail;

public class GetSubmitDetailQuery:IRequest<IResult<GetAllSubmitsResponse>>
{
    public int SubmitId { get; set; }
    public  int FestivalId { get; set; }
}

public class GetSubmitDetailQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IUserService userService)
    : IRequestHandler<GetSubmitDetailQuery, IResult<GetAllSubmitsResponse>>
{
    public async Task<IResult<GetAllSubmitsResponse>> Handle(GetSubmitDetailQuery request, CancellationToken cancellationToken)
    {
        var submit = await unitOfWork.Repository<Submit>()
            .Entities
            .Where(p => p.Id == request.SubmitId)
            .Include(p => p.Project)
            .Include(p => p.Festival)
            .ProjectTo<GetAllSubmitsResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        var user = await userService.GetAsync(submit.ProjectOwnerId);
        submit.ProjectOwnerFullName = user.Data.FullName;

        return await Result<GetAllSubmitsResponse>.SuccessAsync(submit);
    }
}