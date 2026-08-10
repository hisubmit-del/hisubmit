using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Features.Wrapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;

namespace HiSubmit.Application.Features.FestivalPaymentItems.Queries.GetAll;

public class GetAllFestivalPaymentItemQuery
    :PagedRequest,IRequest<PaginatedResult<GetAllFestivalPaymentItemResponse>>
{
    public  string SearchString { get; set; }
    public  int? FestivalId { get; set; }
    public  RequestAccountType AccountType { get; set; }
}

public class GetAllFestivalPaymentItemQueryHandler
    : IRequestHandler<GetAllFestivalPaymentItemQuery, PaginatedResult<GetAllFestivalPaymentItemResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetAllFestivalPaymentItemQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    public Task<PaginatedResult<GetAllFestivalPaymentItemResponse>> Handle
        (GetAllFestivalPaymentItemQuery request, CancellationToken cancellationToken)
    {
        if (request.AccountType == RequestAccountType.Admin)
        {
            var response = _unitOfWork.Repository<FestivalPaymentItem>()
                .Entities
                .ProjectTo<GetAllFestivalPaymentItemResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request);
            return response;
        }
        else
        {
            var response = _unitOfWork.Repository<FestivalPaymentItem>()
                .Entities
                .Where(p=>p.FestivalId==request.FestivalId.Value)
                .ProjectTo<GetAllFestivalPaymentItemResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request);
            return response;
        }
    }
}