using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.SubUsers.Commands.AddEditRoles;

public class AddEditFestivalRoleCommand : RoleRequest, IRequest<Result<string>>;

public class AddEditFestivalCommandHandler(IRoleService roleService)
    : IRequestHandler<AddEditFestivalRoleCommand, Result<string>>
{
    public Task<Result<string>> Handle(AddEditFestivalRoleCommand request, CancellationToken cancellationToken)
    {
        return roleService.SaveAsync(request);
    }
}