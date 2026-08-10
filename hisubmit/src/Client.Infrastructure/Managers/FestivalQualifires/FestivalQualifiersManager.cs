using Hisubmit.Client.SharedModels.Features.FestivalQualifyers.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.FestivalQualifires
{
    public class FestivalQualifiersManager : IFestivalQualifiersManager
    {
        private readonly HttpClient _httpClient;
        public FestivalQualifiersManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<List<GetAllFestivalQualifiersResponse>>> GetAllAsync(GetAllFestivalQualifiersQuery query)
        {
            var response =await _httpClient
                .GetAsync(FestivalQualifiresEndPoint.GetAll(query));
            return await response.ToResult<List<GetAllFestivalQualifiersResponse>>();
        }
    }
}

