using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Contents;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.News.Queries;

public class GetAllNewQuery : GetAllNewRequest, IRequest<PaginatedResult<GetAllNewResponse>>;

public class GetAllNewQueryHandler(
    IUnitOfWork<int> unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetAllNewQuery, PaginatedResult<GetAllNewResponse>>
{


    public async Task<PaginatedResult<GetAllNewResponse>> Handle(GetAllNewQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetAllNewFilterSpecification(request.SearchString, request.IsEnable, request.FestivalId, request.GetFestivalNews);
        var response = await unitOfWork.Repository<New>()
            .Entities
            .Specify(specification)
            .ProjectTo<GetAllNewResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        if (request.ReturnLastNews)
        {
            var last = await unitOfWork.Repository<New>().Entities.LastOrDefaultAsync(cancellationToken);
            if (last != null)
            {
                var mapped = mapper.Map<GetAllNewResponse>(last);
                mapped.IsPined=true;
                response.Data.Add
                    (mapped);

            }
        }
        return response;
    }
}

