using System;
//using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Constants.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Authentication
{
    public class AuthenticationHeaderHandler : DelegatingHandler
    {
        //private readonly ILocalStorageService localStorage;

        //public AuthenticationHeaderHandler(ILocalStorageService localStorage)
        //    => this.localStorage = localStorage;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization?.Scheme != "Bearer")
            {
                //var savedToken = await localStorage
                //    .GetItemAsync<string>(StorageConstants.Local.AuthToken);

                //var expiresDate = await localStorage
                //    .GetItemAsync<DateTime?>(StorageConstants.Local.ExpireToken);
                
                //if (!string.IsNullOrWhiteSpace(savedToken) && 
                //    expiresDate!=null && expiresDate>DateTime.Now)
                //{
                //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                //}
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}