using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.Requests.Identity;
using HiSubmit.Client.Infrastructure.Managers.Identity.Authentication;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Client.SharedModels.Constants.Role;
using System.Security.Claims;
using System.Text.Json;
using IResult = HiSubmit.Client.SharedModels.Wrapper.IResult;
using TokenRequest = Hisubmit.Client.SharedModels.Requests.Identity.TokenRequest;
using TokenResponse = Hisubmit.Client.SharedModels.Responses.Identity.TokenResponse;

namespace Web.Settings;

public class ServerAuthenticationManager(HttpClient httpClient,IHttpContextAccessor httpContextAccessor)
    :IAuthenticationManager
{
    

    public async Task<IResult<TokenResponse>> Login(TokenRequest model)
    {
        var result = await httpClient.PostAsJsonAsync("api/identity/token/login", model);
        return await result.ToResult<TokenResponse>();
    }

    public async Task<IResult> Logout()
    {
        return await Result.SuccessAsync();
    }

    public async Task<string> RefreshToken()
    {
       return  String.Empty;
    }

    public async Task<string> TryRefreshToken()
    {
        return  String.Empty;
    }

    public async Task<string> TryForceRefreshToken()
    {
        return  String.Empty;
    }

    public async Task<ClaimsPrincipal> CurrentUser()
    {
        if (httpContextAccessor.HttpContext != null)
        {
            var currentUser =  httpContextAccessor.HttpContext.User;

            return currentUser;
        }

        return new ClaimsPrincipal(){Claims = {  },Identities = {  }};
    }

    public Task<IResult> VerifyEmail(VerificationCodeRequest codeRequest)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> ResendVerifyEmail(ResendVerificationCodeRequest codeRequest)
    {
        throw new NotImplementedException();
    }

    public int? GetMainFestivalId()
    {
        var f =  httpContextAccessor.HttpContext?.User?.FindFirst(ApplicationClaimTypes.FestivalId);
        if(f!=null)
            return int.Parse(f.Value);
        return null;
    }

    public IEnumerable<int> GetOtherFestivalId()
    {
        return LoadOtherFestivalPermissions().Select(p => p.Key);
    }

    public Task<int?> GetSelectedFestivalId()
    {
        if (httpContextAccessor.HttpContext != null
            && httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(ApplicationClaimTypes.SelectedFestival))
        {
            var festivalId =  httpContextAccessor.HttpContext.Request.Cookies[ApplicationClaimTypes.SelectedFestival];
            if (int.TryParse(festivalId, out var parsedFestivalId) && parsedFestivalId > 0)
                return Task.FromResult<int?>(parsedFestivalId);
        }

        return Task.FromResult<int?>(null);
    }

    public Task<bool> IsPersonalAccountSelected()
    {
        if (httpContextAccessor.HttpContext != null
            && httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(
                ApplicationClaimTypes.SelectedFestival, out var festivalId))
        {
            return Task.FromResult(string.Equals(festivalId, "0", StringComparison.Ordinal));
        }

        var user = httpContextAccessor.HttpContext?.User;
        if (user?.IsInRole(RoleConstants.AdministratorRole) == true)
            return Task.FromResult(false);

        // Artist-only accounts start in the personal workspace when no
        // explicit account-selection cookie exists.
        return Task.FromResult(GetMainFestivalId() is null);
    }

    public Task<int?> GetAdminLoginToFestivalId()
    {
        if (httpContextAccessor.HttpContext != null
            && httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(ApplicationClaimTypes.AdminLoginFestival))
        {
            var festivalId = httpContextAccessor.HttpContext.Request.Cookies[ApplicationClaimTypes.AdminLoginFestival];
            return Task.FromResult<int?>(int.Parse(festivalId));
        }

        return Task.FromResult<int?>(null);
    }

    private  Dictionary<int, string[]>? LoadOtherFestivalPermissions()
    {
        if (httpContextAccessor.HttpContext != null)
        {
            var currentUser = httpContextAccessor.HttpContext.User;

            var permissionsClaims = currentUser.Claims
                .FirstOrDefault(p => p.Type==ApplicationClaimTypes.FestivalPermission)?.Value;

            if (!string.IsNullOrEmpty(permissionsClaims))
            {
                var permissions = JsonSerializer.Deserialize<Dictionary<int, string[]>>(permissionsClaims);
                return permissions; 
            }
        }

        return new Dictionary<int, string[]>();
    }
}
