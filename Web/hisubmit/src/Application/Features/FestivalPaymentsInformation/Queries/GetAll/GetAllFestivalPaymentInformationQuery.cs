using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;

namespace HiSubmit.Application.Features.FestivalPaymentsInformation.Queries.GetAll;

public class GetAllFestivalPaymentInformationQuery
    :PagedRequest,IRequest<PaginatedResult<GetAllFestivalPaymentInformationResponse>>
{
    
}

public class GetAllFestivalPaymentInformationQueryHandler
    : IRequestHandler<GetAllFestivalPaymentInformationQuery, 
        PaginatedResult<GetAllFestivalPaymentInformationResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetAllFestivalPaymentInformationQueryHandler
        (IMapper mapper, IUnitOfWork<int> unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    public async Task<PaginatedResult<GetAllFestivalPaymentInformationResponse>> Handle
        (GetAllFestivalPaymentInformationQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Repository<FestivalPaymentInformation>()
            .Entities
            .ProjectTo<GetAllFestivalPaymentInformationResponse>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);
        return result;
    }
}