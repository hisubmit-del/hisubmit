using AutoMapper;
using HiSubmit.Application.Exceptions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.SubUsers.Queries.GetFestivalUsers
{
    public class GetFestivalSubUserQuery:IRequest<Result<List<UserResponse>>>
    {
        public int FestivalId { get; set; }
    }

    public class GetFestivalSubUserQueryHandler : IRequestHandler<GetFestivalSubUserQuery, Result<List<UserResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public GetFestivalSubUserQueryHandler(
            IUnitOfWork<int> unitOfWork,IMapper mapper,
            IUserService userService,ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<UserResponse>>> Handle(GetFestivalSubUserQuery request, CancellationToken cancellationToken)
        {
            var usersId = await _unitOfWork.Repository<FestivalSubUser>()
                .Entities.Where(p => p.FestivalId == request.FestivalId)
                .Select(p => p.UserId)
                .ToListAsync(cancellationToken);

            var users =await _userService.GetAllAsync( usersId);
            return users;
        }
    }
}
