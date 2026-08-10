using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Features.Advertises.Commands;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Features.Advertises.Queries;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Advertise;
using HiSubmit.Domain.Enums.Advertises;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;

namespace HiSubmit.Application.Features.Advertises.Queries;

public class GetAllAdvertiseQuery:
    GetAllAdvertiseRequest,IRequest<PaginatedResult<GetAllAdvertiseResponse>>;

public  class  GetAllAdvertiseQueryHandler:IRequestHandler<GetAllAdvertiseQuery,PaginatedResult<GetAllAdvertiseResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IUserService _userService;

    public GetAllAdvertiseQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork,IUserService userService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userService = userService;
    }
    public async Task<PaginatedResult<GetAllAdvertiseResponse>> Handle(GetAllAdvertiseQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.Repository<AdvertiseRequest>()
            .Entities
            .ProjectTo<GetAllAdvertiseResponse>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        var users = await  _userService.GetAllAsync(result.Data.Select(p => p.UserId).ToList());
        foreach (var user in users.Data)
        {
            var advertises = result.Data.Where(p => p.UserId == user.Id);
            foreach (var r in advertises)
            {
                r.UserName = user.FullName;
            } 
        }
        return result;
    }
}

