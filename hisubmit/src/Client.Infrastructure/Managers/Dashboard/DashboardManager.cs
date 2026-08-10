using Hisubmit.Client.SharedModels.Features.Dashboards.Queries.GetData;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Net.Http;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Dashboard
{
    public class DashboardManager : IDashboardManager
    {
        private readonly HttpClient _httpClient;

        public DashboardManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<DashboardDataResponse>> GetDataAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DashboardEndpoints.GetData);
            var data = await response.ToResult<DashboardDataResponse>();
            return data;
        }
    }
}