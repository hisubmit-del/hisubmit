using HiSubmit.Application.Interfaces.Common;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiSubmit.Application.Interfaces.Services.Identity
{
    public interface IRoleClaimService : IService
    {
        Task<Result<List<RoleClaimResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleClaimResponse>> GetByIdAsync(int id);

        Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId);

        Task<Result<string>> SaveAsync(RoleClaimRequest request);

        Task<Result<string>> DeleteAsync(int id);
    }
}