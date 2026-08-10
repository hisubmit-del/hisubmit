using Hisubmit.Client.SharedModels.Features.FestivalFocs.Commands.AddEditFestivalFocus;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Commands.DeleteFestivalFocus;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Queries.GetAllFestivalFocus;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Queries.GetFestivalFocusDetail;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Catalogs;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Catalog.FestivalFocus
{
    public class FestivalFocusManager : IFestivalFocusManager
    {
        private readonly HttpClient _httpClient;
        public FestivalFocusManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IResult<int>> DeleteAsync(DeleteFestivalFocusCommand command)
        {
            var response =await _httpClient.DeleteAsync(FestivalFocusEndPoint.DeleteFestivalFocus(command));
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllFestivalFocusResponse>>> GetAllAsync(GetAllFestivalFocusQuery query)
        {
            var response = await _httpClient.GetAsync(FestivalFocusEndPoint.GetAllFestivalFocus(query));
            return await response.ToResult<List<GetAllFestivalFocusResponse>>();
        }



        public async Task<IResult<GetFestivalFocusDetailResponse>> GetById(GetFestivalFocusDeailQuery query)
        {
            var response = await _httpClient.GetAsync(FestivalFocusEndPoint.GetFestivalFocusDetaiil(query));
            return await response.ToResult<GetFestivalFocusDetailResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditFestivalFocusCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(FestivalFocusEndPoint.UppdateEventCategory,request);
            return await response.ToResult<int>();
        }
    }
}
