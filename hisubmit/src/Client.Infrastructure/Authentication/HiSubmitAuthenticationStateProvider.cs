using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;




//using Blazored.LocalStorage;
//using Hisubmit.Client.SharedModels.Contracts.Permission;
//using Hisubmit.Client.SharedModels.Constants.Storage;
//using Microsoft.AspNetCore.Components.Authorization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Security.Claims;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace HiSubmit.Client.Infrastructure.Authentication;

//public class HiSubmitAuthenticationStateProvider : AuthenticationStateProvider
//{
//    private readonly HttpClient _httpClient;
//    private readonly ILocalStorageService _localStorage;


//    public HiSubmitAuthenticationStateProvider(
//        HttpClient httpClient,
//        ILocalStorageService localStorage)
//    {
//        _httpClient = httpClient;
//        _localStorage = localStorage;
//        GetFestivalId();
//    }

//    public void MarkUserAsAuthenticated(string userName)
//    {
//        var authenticatedUser = new ClaimsPrincipal(
//            new ClaimsIdentity(new[]
//            {
//                new Claim(ClaimTypes.Name, userName)
//            }, "apiauth"));

//        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

//        NotifyAuthenticationStateChanged(authState);
//    }

//    public void MarkUserAsLoggedOut()
//    {
//        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
//        var authState = Task.FromResult(new AuthenticationState(anonymousUser));

//        NotifyAuthenticationStateChanged(authState);
//    }

//    public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
//    {
//        var state = await GetAuthenticationStateAsync();
//        var authenticationStateProviderUser = state.User;
//        return authenticationStateProviderUser;
//    }

//    private ClaimsPrincipal AuthenticationStateUser { get; set; }

//    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//    {
//        var savedToken = await _localStorage.GetItemAsync<string>
//            (StorageConstants.Local.AuthToken);
//        DateTime? expiresDate = null;
//        if (await _localStorage.ContainKeyAsync(StorageConstants.Local.ExpireToken))
//        {
//            expiresDate = await _localStorage.GetItemAsync<DateTime?>
//                (StorageConstants.Local.ExpireToken);
//        }

//        if (!string.IsNullOrWhiteSpace(savedToken) &&
//            expiresDate != null && expiresDate > DateTime.Now)
//        {
//            _httpClient.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", savedToken);
//            var state = new AuthenticationState(
//                new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromJwt(savedToken), "jwt")));
//            AuthenticationStateUser = state.User;
//            NotifyAuthenticationStateChanged(Task.FromResult(state));
//            return state;
//        }

//        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//    }

//    private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
//    {
//        var claims = new List<Claim>();
//        var payload = jwt.Split('.')[1];
//        var jsonBytes = ParseBase64WithoutPadding(payload);
//        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

//        if (keyValuePairs != null)
//        {
//            #region Get Roles

//            keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);
//            if (roles != null)
//            {
//                if (roles.ToString()!.Trim().StartsWith("["))
//                {
//                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);
//                    claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
//                }
//                else
//                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));

//                keyValuePairs.Remove(ClaimTypes.Role);
//            }

//            #endregion

//            #region Get FestivalId Roles

//            keyValuePairs.TryGetValue(ApplicationClaimTypes.FestivalRole, out var festivalRoles);

//            if (festivalRoles != null)
//            {
//                if (festivalRoles.ToString()!.Trim().StartsWith("["))
//                {
//                    var parsedRoles = JsonSerializer.Deserialize<string[]>(festivalRoles.ToString()!);
//                    claims.AddRange(parsedRoles.Select(role =>
//                        new Claim(ApplicationClaimTypes.FestivalRole, role)));
//                }
//                else
//                    claims.Add(new Claim(ApplicationClaimTypes.FestivalRole, festivalRoles.ToString()!));

//                keyValuePairs.Remove(ApplicationClaimTypes.FestivalRole);
//            }

//            #endregion

//            #region Get Permissions

//            keyValuePairs.TryGetValue(ApplicationClaimTypes.Permission, out var permissions);
//            if (permissions != null)
//            {
//                if (permissions.ToString()!.Trim().StartsWith("["))
//                {
//                    var parsedPermissions = JsonSerializer.Deserialize<string[]>(permissions.ToString()!);
//                    claims.AddRange(parsedPermissions.Select(permission =>
//                        new Claim(ApplicationClaimTypes.Permission, permission)));
//                }
//                else
//                    claims.Add(new Claim(ApplicationClaimTypes.Permission, permissions.ToString()!));

//                keyValuePairs.Remove(ApplicationClaimTypes.Permission);
//            }

//            #endregion

//            #region Get FestivalId Permissins

//            keyValuePairs.TryGetValue(ApplicationClaimTypes.FestivalPermission, out var festivalPermissions);
//            if (festivalPermissions != null)
//            {
//                if (festivalPermissions.ToString()!.Trim().StartsWith("["))
//                {
//                    var parsedPermissions = JsonSerializer.Deserialize<string[]>(festivalPermissions.ToString()!);
//                    claims.AddRange(parsedPermissions
//                        .Select(fPs =>
//                            new Claim(ApplicationClaimTypes.Permission, fPs.ToString().Split("-").Last())));

//                    claims.AddRange(parsedPermissions
//                        .Select(fPs =>
//                            new Claim(ApplicationClaimTypes.FestivalPermission, fPs.ToString())));
//                }
//                else
//                {
//                    var claimName = festivalPermissions.ToString()!.Split('-').Last();
//                    claims.Add(new Claim(ApplicationClaimTypes.Permission, claimName));
//                    claims.Add(new Claim(ApplicationClaimTypes.FestivalPermission,
//                        festivalPermissions.ToString()!));
//                }

//                keyValuePairs.Remove(ApplicationClaimTypes.FestivalPermission);
//            }

//            #endregion


//            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));
//        }

//        return claims;
//    }

//    private async Task<int> GetFestivalId()
//    {
//        var jwt = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
//        if (jwt == null)
//            return 0;
//        var payload = jwt.Split('.')[1];
//        var jsonBytes = ParseBase64WithoutPadding(payload);
//        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

//        if (keyValuePairs == null) return 0;

//        keyValuePairs.TryGetValue("FestivalId", out var festivalId);
//        return int.Parse((string)festivalId ?? string.Empty);
//    }

//    private byte[] ParseBase64WithoutPadding(string base64)
//    {
//        switch (base64.Length % 4)
//        {
//            case 2:
//                base64 += "==";
//                break;
//            case 3:
//                base64 += "=";
//                break;
//        }

//        return Convert.FromBase64String(base64);
//    }
//}

