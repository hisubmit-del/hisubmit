using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Requests.Identity;
using Hisubmit.Client.SharedModels.Responses.Identity;

namespace HiSubmit.Client.Infrastructure.Managers.Identity.RoleClaims
{
    public interface IRoleClaimManager : ITransientManager
    {
        Task<IResult<List<RoleClaimResponse>>> GetRoleClaimsAsync();

        Task<IResult<List<RoleClaimResponse>>> GetRoleClaimsByRoleIdAsync(string roleId);

        Task<IResult<string>> SaveAsync(RoleClaimRequest role);

        Task<IResult<string>> DeleteAsync(string id);
    }
}