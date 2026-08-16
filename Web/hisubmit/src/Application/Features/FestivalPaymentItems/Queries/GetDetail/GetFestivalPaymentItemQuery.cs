using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.FestivalPaymentItems.Queries.GetDetail;

public class GetFestivalPaymentItemDetailQuery
    :IRequest<IResult<GetFestivalPaymentItemDetailResponse>>
{
    public int Id { get; set; }
}

public class GetFestivalPaymentItemDetailQueryHandler
    : IRequestHandler<GetFestivalPaymentItemDetailQuery, IResult<GetFestivalPaymentItemDetailResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetFestivalPaymentItemDetailQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IResult<GetFestivalPaymentItemDetailResponse>> 
        Handle(GetFestivalPaymentItemDetailQuery request, CancellationToken cancellationToken)
    {
        var info = await _unitOfWork.Repository<FestivalPaymentItem>()
            .Entities
            .Where(p => p.Id == request.Id)
            .ProjectTo<GetFestivalPaymentItemDetailResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        return await Result<GetFestivalPaymentItemDetailResponse>.SuccessAsync(info);
    }
}