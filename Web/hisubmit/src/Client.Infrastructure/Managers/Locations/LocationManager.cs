//using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Features.Locatuions.Countries.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Locations
{
    public class LocationManager : ILocationManager
    {
        //private ILocalStorageService _localStorageService;
        private HttpClient _httpClient;
        public LocationManager (HttpClient httpClient
           // , 
           // ILocalStorageService localStorageService
            )
        {
            _httpClient = httpClient;
           // _localStorageService = localStorageService;
        }

        public async Task<Result<List<GetAllCountryResponse>>> GetAllCountryAsync(GetAllCountryQuery query)
        {
            // if (await _localStorageService.ContainKeyAsync("Countries"))
            // {
            //     var cachCountries = await _localStorageService.GetItemAsync<Result<List<GetAllCountryResponse>>>("Countries");
            //     if (cachCountries.Succeeded)
            //     {
            //         return cachCountries;
            //     }
            // }

            var response = await _httpClient.GetAsync(LocationEndpoints.GetAllCountries(query));
            var countriesResponse = await response.ToResult2<List<GetAllCountryResponse>>();
            // await _localStorageService.SetItemAsync("Countries", countriesResponse);  
            return countriesResponse;
        }
    }
}
