using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Filters;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.FestivalPaymentsInformation.Queries.GetDetail;

public class GetFestivalPaymentInformationDetailQuery
    : IRequest<IResult<GetFestivalPaymentInformationDetailResponse>>
{
    public int FestivalId { get; set; }
    
}

public class GetFestivalPaymentInformationDetailQueryHandler
:IRequestHandler<GetFestivalPaymentInformationDetailQuery,
    IResult<GetFestivalPaymentInformationDetailResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICheckPermission _checkPermission;

    public GetFestivalPaymentInformationDetailQueryHandler
        (IMapper mapper, IUnitOfWork<int> unitOfWork,
            ICheckPermission checkPermission)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _checkPermission = checkPermission;
    }
    public async Task<IResult<GetFestivalPaymentInformationDetailResponse>> Handle
        (GetFestivalPaymentInformationDetailQuery request, CancellationToken cancellationToken)
    {
        var info = await _unitOfWork.Repository<FestivalPaymentInformation>()
            .Entities
            .Where(p => p.FestivalId == request.FestivalId)
            .ProjectTo<GetFestivalPaymentInformationDetailResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        return await Result<GetFestivalPaymentInformationDetailResponse>.SuccessAsync(info);
    }
}