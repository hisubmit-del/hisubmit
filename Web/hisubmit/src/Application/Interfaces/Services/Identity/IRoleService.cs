using HiSubmit.Application.Interfaces.Common;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.CustomeAttribute;

namespace HiSubmit.Application.Interfaces.Services.Identity
{
    public interface IRoleService : IService
    {
        Task<Result<List<RoleResponse>>> GetAllAsync(int? festivalId=null);

        Task<int> GetCountAsync();

        Task<Result<RoleResponse>> GetByIdAsync(string id);

        Task<Result<string>> SaveAsync(RoleRequest request);

        Task<Result<string>> DeleteAsync(string id);

        Task<Result<PermissionResponse>> GetAllPermissionsAsync(string roleId,PermissionType? permissionType);

        Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request);
    }

    public class GetAllPermissionRequest
    {
        public string RoleId { get; set; }
        public  PermissionType? PermissionType { get; set; }
    }
}

