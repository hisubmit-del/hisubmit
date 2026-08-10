using Hisubmit.Client.SharedModels.Requests.Identity;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Client.Infrastructure.Managers.Identity.Authentication;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using TokenRequest = Hisubmit.Client.SharedModels.Requests.Identity.TokenRequest;
using TokenResponse = Hisubmit.Client.SharedModels.Responses.Identity.TokenResponse;

namespace HiSubmit.Server.Settings;

public class ServerAuthenticationManager(HttpClient httpClient)
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
        return new ClaimsPrincipal()
        {
            Claims = { },
            Identities = { }
        };
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
        throw new NotImplementedException();
    }

    public IEnumerable<int> GetOtherFestivalId()
    {
        throw new NotImplementedException();
    }

    public Task<int?> GetSelectedFestivalId()
    {
        throw new NotImplementedException();
    }

    public Task<int?> GetAdminLoginToFestivalId()
    {
        throw new NotImplementedException();
    }
}