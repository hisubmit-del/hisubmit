//using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Features.MediaRights.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Catalogs;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.MediaRights
{
    public interface IMediaRightManager : ITransientManager
    {
        Task<Result<List<GetAllMediaRightResponse>>> GetAllAsync(GetAllMediaRightQuery query);
    }
    public class MediaRightManager : IMediaRightManager
    {

        private HttpClient _httpClient;
        //private ILocalStorageService _localStorageService;

        public MediaRightManager(HttpClient httpClient
            //ILocalStorageService localStorageService
            )
        {
            _httpClient = httpClient;
            //_localStorageService = localStorageService;
        }
        public async Task<Result<List<GetAllMediaRightResponse>>> GetAllAsync(GetAllMediaRightQuery query)
        {
            // if (await _localStorageService.ContainKeyAsync("MediaRights"))
            // {
            //     var cachedRights = await _localStorageService.GetItemAsync<Result<List<GetAllMediaRightResponse>>>("MediaRights");
            //     if (cachedRights != null && cachedRights.Succeeded)
            //     {
            //         return cachedRights;
            //     }
            // }
            var response = await _httpClient.GetAsync(MediaRightEndPoint.GetAll(query));
            var responseRights = await response.ToResult2<List<GetAllMediaRightResponse>>();
          // await _localStorageService.SetItemAsync("MediaRights", responseRights);
            return responseRights;
        }
    }
}
