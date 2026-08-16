using HiSubmit.Application.Interfaces.Common;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Interfaces.Services.Identity
{
    public interface IUserService : IService
    {
        Task<Result<List<UserResponse>>> GetAllAsync(List<string> usersId=null);

        Task<int> GetCountAsync();

        Task<IResult<UserResponse>> GetAsync(string userId);

        Task<Result<RegisterUserResponse>> RegisterAsync(RegisterRequest request, string origin);

        Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);

        Task<IResult<UserRolesResponse>> GetRolesAsync(string id,int? festivalId=null);

        Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);

        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);

        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<string> ExportToExcelAsync(string searchString = "");
        Task<IResult> AddToRoleAsync(string userId, List<string> rolesName);
        Task<IResult> AddToRoleAsync(string userId,string roleId);

        Task<Dictionary<string, string>> GetUserName(List<string> ids);
        Task<Dictionary<string, UserResponse>> GetUser(List<string> ids);
        
        Task<FeeStatus> GetUserType(string userId);
        Task<string> GetUserByEmailAddress(string emailAddress);
        Task ChangeAccountStatus(FeeStatus status,string userId);
        Task<List<UserResponse>> GetAllAdminUsers();
        Task AddClaim(string userId,Claim claim);
        Task AddClaims(string userId,List<Claim> claims);

    }
}