using System;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
//using Blazored.LocalStorage;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Requests.Identity;
using Hisubmit.Client.SharedModels.Responses.Identity;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.Extensions.Localization;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace HiSubmit.Client.Infrastructure.Managers.Identity.Authentication
{
    public class ClientAuthenticationManager(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authenticationStateProvider,
        IStringLocalizer<ClientAuthenticationManager> localize)
        : IAuthenticationManager
    {
        public async Task<ClaimsPrincipal> CurrentUser()
        {
            var state = await authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User;
        }

        public async Task<IResult> VerifyEmail(VerificationCodeRequest codeRequest)
        {
            var res =await httpClient.PostAsJsonAsync(TokenEndpoints.Verify,codeRequest);
            return await res.ToResult<IResult>();
        }

        public async Task<IResult> ResendVerifyEmail(ResendVerificationCodeRequest codeRequest)
        {
            var res = await httpClient.PostAsJsonAsync(TokenEndpoints.Resend, codeRequest);
            return await res.ToResult<IResult>();
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

        public async Task<IResult<TokenResponse>> Login(TokenRequest model)
        {
            var response = await httpClient.PostAsJsonAsync(TokenEndpoints.Get, model);
            var result = await response.ToResult<TokenResponse>();
            if (result.Succeeded)
            {
                var token = result.Data.Token;
                var refreshToken = result.Data.RefreshToken;
                var userImageUrl = result.Data.UserImageURL;
                var expireToken = result.Data.TokenExpiryTime;
                var festivalId = GetFestivalFromJwttoken(token);

                //await localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
                //await localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
                //await localStorage.SetItemAsync(StorageConstants.Local.FestivalId, festivalId);
                //await localStorage.SetItemAsync(StorageConstants.Local.ExpireToken, expireToken);

                if (festivalId != 0)
                  //  await localStorage.SetItemAsync(StorageConstants.Local.SelectedFestivalId, festivalId);

                if (!string.IsNullOrEmpty(userImageUrl))
                {
                  //  await localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, userImageUrl);
                }

                //((HiSubmitAuthenticationStateProvider)authenticationStateProvider)
                //    .MarkUserAsAuthenticated(model.Email);
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await Result<TokenResponse>.SuccessAsync(result.Data);
            }

            return await Result<TokenResponse>.FailAsync(result.Messages[0],result.Data);
        }

        public async Task<IResult> Logout()
        {
            //await localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
            //await localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
            //await localStorage.RemoveItemAsync(StorageConstants.Local.UserImageURL);
            //await localStorage.RemoveItemAsync(StorageConstants.Local.FestivalId);
            //await localStorage.RemoveItemAsync(StorageConstants.Local.ExpireToken);
            //((HiSubmitAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
            httpClient.DefaultRequestHeaders.Authorization = null;
            return await Result.SuccessAsync();
        }

        public async Task<string> RefreshToken()
        {
            throw new Exception();
            //var token = await localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            //var refreshToken = await localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);

            //var response = await httpClient
            //    .PostAsJsonAsync(TokenEndpoints.Refresh, new RefreshTokenRequest 
            //        { Token = token,
            //            RefreshToken = refreshToken });

           // var result = await response.ToResult<TokenResponse>();

            //if (!result.Succeeded)
            //{
            //    throw new ApplicationException(localize["Something went wrong during the refresh token action"]);
            //}

           // token = result.Data.Token;
           // refreshToken = result.Data.RefreshToken;
        //    var expiresToken = result.Data.TokenExpiryTime;
            //await localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
            //await localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
            //await localStorage.SetItemAsync(StorageConstants.Local.ExpireToken, expiresToken);
            //await localStorage.SetItemAsync(StorageConstants.Local.FestivalId, GetFestivalFromJwttoken(token));

            //httpClient.DefaultRequestHeaders.Authorization = 
            //    new AuthenticationHeaderValue("Bearer", token);
            //return token;
        }

        public async Task<string> TryRefreshToken()
        {
            //check if token exists
            var availableToken = await localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
            if (string.IsNullOrEmpty(availableToken)) return string.Empty;
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
            Console.WriteLine(exp);
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            Console.WriteLine(expTime);
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;
            if (diff.TotalMinutes <= 1)
                return await RefreshToken();
            return string.Empty;
        }

        public async Task<string> TryForceRefreshToken()
        {
            return await RefreshToken();
        }



        private int GetFestivalFromJwttoken(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValue = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            
            if(keyValue != null)
            {
                keyValue.TryGetValue("FestivalId", out var jsonFestivalId);
                if(jsonFestivalId != null)
                {
                   var festivalId= JsonSerializer.Deserialize<int>(jsonFestivalId.ToString());

                    return festivalId;
                }
            }
            return 0;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}