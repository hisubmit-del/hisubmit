using HiSubmit.Application.Exceptions;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.SubUsers.GetFestivalRoles
{
    public class GetFestivalRolesQuery:IRequest<Result<List<RoleResponse>>>
    {
        public int FestivalId { get; set; }
    }
    
    public class GetFestivalRolesQueryHandler : IRequestHandler<GetFestivalRolesQuery, Result<List<RoleResponse>>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRoleService _roleService;
        public GetFestivalRolesQueryHandler(ICurrentUserService currentUserService,IRoleService roleService)
        {
            _currentUserService = currentUserService;
            _roleService = roleService;
        }
        public async Task<Result<List<RoleResponse>>> Handle(GetFestivalRolesQuery request, CancellationToken cancellationToken)
        {
            var roles =await  _roleService.GetAllAsync(request.FestivalId);
            return roles;
        }
    }
}
