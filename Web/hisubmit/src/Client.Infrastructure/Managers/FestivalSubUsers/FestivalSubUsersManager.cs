using Hisubmit.Client.SharedModels.Features.SubUsers.Commands.AddEditRoles;
using Hisubmit.Client.SharedModels.Features.SubUsers.Queries.GetFestivalUsers;
using Hisubmit.Client.SharedModels.Features.Users.Commands.Register;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.RemovedUserFromProject;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.RemoveUserFromFestival;
using Hisubmit.Client.SharedModels.Features.SubUsers.Commands.AddExistingUserToFestival;
using Hisubmit.Client.SharedModels.Features.SubUsers.GetFestivalRoles;
using Hisubmit.Client.SharedModels.Requests.Identity;
using Hisubmit.Client.SharedModels.Responses.Identity;

namespace HiSubmit.Client.Infrastructure.Managers.FestivalSubUsers
{
    public interface IFestivalSubUserManager:ITransientManager
    {
        Task<IResult<List<RoleResponse>>> GetFestivalRoleAsync(int festivalId);
        Task<IResult<string>> SaveRole(AddEditFestivalRoleRequest request);
        Task<IResult<string>> SaveUser(RegisterUserCommand command);
        Task<IResult<List<UserResponse>>> GetFestivalUserAsync(GetFestivalSubUserQuery query);
        Task<IResult> AddExistingUserToFestival(AddExistingUserToFestivalCommand command);
        Task<IResult<PermissionResponse>> GetAllPermission(string roleId);
        Task<IResult<string>> UpdatePermissionsAsync(PermissionRequest request);
        Task<IResult<UserRolesResponse>>GetUserRolesAsync(string userId,int? festivalId );
        Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);
        Task<IResult> RemovedUserFromFestival(RemoveUserFromFestivalCommand command,int festivalId);
        Task<IResult> RemovedUserFromProject(RemovedUserFromProjectCommand command,int festivalId);

    }
    public class FestivalSubUserManager : IFestivalSubUserManager
    {
        private BaseEndPoint _endPoint;
        private HttpClient _httpClient;
        public FestivalSubUserManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _endPoint = new BaseEndPoint("api/v1/FestivalSubUsers");
        }

        public async Task<IResult<List<RoleResponse>>> GetFestivalRoleAsync(int festivalId)
        {
            var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{festivalId}/FestivalRoles",new GetFestivalRolesQuery(){FestivalId = festivalId}));
            return await response.ToResult<List<RoleResponse>>();
        }

        public async Task<IResult<List<UserResponse>>> GetFestivalUserAsync(GetFestivalSubUserQuery query)
        {
            var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{0}/FestivalUser",query));
            return await response.ToResult<List<UserResponse>>();
        }

        public async Task<IResult> AddExistingUserToFestival(AddExistingUserToFestivalCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl($"{0}/AddExistingUser"),command);
            return await response.ToResult<string>();
        }

        public async Task<IResult<PermissionResponse>> GetAllPermission(string roleId)
        {
            var action = $"{0}/RolePermission?roleId={roleId}";
            var response = await _httpClient.GetAsync(_endPoint.GenerateUrl(action));
            return await response.ToResult<PermissionResponse>();
        }

        public async Task<IResult<string>> UpdatePermissionsAsync(PermissionRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(_endPoint.GenerateUrl($"{0}/updatePermission"), request);
            return await response.ToResult<string>();
        }

        public async Task<IResult<UserRolesResponse>> GetUserRolesAsync(string userId, int? festivalId)
        {
            var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{festivalId}/UserRoles?userId={userId}&fId={festivalId}"));
            return await response.ToResult<UserRolesResponse>();
        }

        public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(_endPoint.GenerateUrl($"{0}/UpdateUserRoles"), request);
            return await response.ToResult<UserRolesResponse>();
        }

        public async Task<IResult> RemovedUserFromFestival(RemoveUserFromFestivalCommand command,int festivalId)
        {
            var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl($"{festivalId}/RemovedUserFromFestival"),command);
            return await response.ToResult();
        }

        public async Task<IResult> RemovedUserFromProject(RemovedUserFromProjectCommand command,int festivalId)
        {
            var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl($"{festivalId}/RemovedUserFromProject"),command);
            return await response.ToResult();
        }

        public async Task<IResult<string>> SaveRole(AddEditFestivalRoleRequest request)
        {
            var url = _endPoint.GenerateUrl($"{request.FestivalId}/SaveRole");

            var response = await _httpClient.PostAsJsonAsync(url,request);

            return await response.ToResult<string>();
        }

        public async Task<IResult<string>> SaveUser(RegisterUserCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl($"{0}/SaveUser"), command);
            return await response.ToResult<string>();
        }
    }
}
