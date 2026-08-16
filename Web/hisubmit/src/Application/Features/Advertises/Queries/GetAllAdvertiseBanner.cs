using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Requests;
using HiSubmit.Application.Specifications.Advertises;
using Hisubmit.Client.SharedModels.Features.Advertises.Queries;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Advertise;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;

namespace HiSubmit.Application.Features.Advertises.Queries;

public class GetAllAdvertiseBannerQuery:
    GetAllAdvertiseBannerRequest, IRequest<PaginatedResult<GetAllAdvertiseBannerResponse>>;

public class GetAllAdvertiseBannerQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork) :
    IRequestHandler<GetAllAdvertiseBannerQuery, PaginatedResult<GetAllAdvertiseBannerResponse>>
{
    public async Task<PaginatedResult<GetAllAdvertiseBannerResponse>> Handle
        (GetAllAdvertiseBannerQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetAllAdvertiseBannerSpecification(request);
        var response = await unitOfWork.Repository<AdvertiseBanner>()
            .Entities
            .Specify(specification)
            .ProjectTo<GetAllAdvertiseBannerResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        return response;
    }
}