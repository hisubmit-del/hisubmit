using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Features.Payments.Queries;
using HiSubmit.Application.Features.Wrapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Identity;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.SoldProducts.Queries;

public class GetSoldProductDetailQuery : IRequest<IResult<GetSoldProductDetailResponse>>
{
    public int Id { get; set; }
    public int? FestivalId { get; set; }
}

public class GetSoldProductDetailQueryHandler
    : IRequestHandler<GetSoldProductDetailQuery, IResult<GetSoldProductDetailResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetSoldProductDetailQueryHandler
        (IMapper mapper, IUserService userService,
            IUnitOfWork<int> unitOfWork, ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userService = userService;
        _currentUserService = currentUserService;
    }

    public async Task<IResult<GetSoldProductDetailResponse>> Handle
        (GetSoldProductDetailQuery request, CancellationToken cancellationToken)
    {
        var productSold = await _unitOfWork.Repository<ProductSold>()
            .Entities
            .Include(p => p.Product).ThenInclude(p => p.Festival)
            .Include(p=>p.Address)
            .ProjectTo<GetSoldProductDetailResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        var user =await _userService.GetAsync(productSold.UserId);
        productSold.UserEmail = user.Data.Email;
        productSold.UserName = user.Data.FullName;
        productSold.UserPhoneNumber = user.Data.PhoneNumber;
        
        //get CartItem data
        var cartItem =await _unitOfWork.Repository<CarTItem>()
            .Entities
            .Where(p => p.ProductSoldId == request.Id)
            .Include(p => p.Cart)
            .ProjectTo<GetCartItemResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        productSold.CartItem = cartItem;
            
        return await Result<GetSoldProductDetailResponse>.SuccessAsync(productSold);
    }
}
