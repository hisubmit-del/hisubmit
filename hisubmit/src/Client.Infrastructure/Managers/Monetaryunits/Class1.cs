using Hisubmit.Client.SharedModels.Features.MonetaryUnits.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Catalogs;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Monetaryunits
{

    public interface IMonetaryUnitsManager:ITransientManager
    {
        Task<IResult<List<GetAllMonetaryUnitRespnse>>> GetAllAsync(GetAllMonetaryUnitQuery query);
    }
    public class MonetaryUnitsManager:IMonetaryUnitsManager
    {
        private HttpClient _httpClient;
        public MonetaryUnitsManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<List<GetAllMonetaryUnitRespnse>>> GetAllAsync(GetAllMonetaryUnitQuery query)
        {
            var response = await _httpClient.GetAsync(MonetaryUnitsEndPoints.GetAll(query));
            return await response.ToResult<List<GetAllMonetaryUnitRespnse>>();
        }
    }
}
