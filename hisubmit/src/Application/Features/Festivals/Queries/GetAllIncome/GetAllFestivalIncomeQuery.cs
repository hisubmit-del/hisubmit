using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Application.Interfaces.Repositories;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllIncome;
public class GetAllFestivalIncomeQuery :
    IRequest<IResult<GetAllFestivalIncomeResponse>>
{
    public int FestivalId { get; set; }

}

internal class GetAllFestivalIncomeQueryHandler :
    IRequestHandler<GetAllFestivalIncomeQuery, IResult<GetAllFestivalIncomeResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    public GetAllFestivalIncomeQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult<GetAllFestivalIncomeResponse>> Handle
        (GetAllFestivalIncomeQuery request, CancellationToken cancellationToken)
    {
        var submits = await _unitOfWork.Repository<CarTItem>()
            .Entities
            .Where(p => p.Cart.Paid &&
                        p.Submit.FestivalId == request.FestivalId
                   && p.CartItemType == Domain.Enums.CartItemType.Submit)
            .ProjectTo<GetAllFestivalIncomeItem>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var paidTotal = submits.Sum(item => item.Price);
        return await Result<GetAllFestivalIncomeResponse>.SuccessAsync(
            new GetAllFestivalIncomeResponse
            {
                FestivalId = request.FestivalId,
                TotalPrice = paidTotal,
                PaidTotlaPrice = paidTotal,
                UnPaidTotalPrice = 0
            });

    }
}

