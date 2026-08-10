using Hisubmit.Client.SharedModels.Features.AdminFestival.Commands.UpdateFestivalState;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Admin;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Commands.UpdateFestivalFeeStatus;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Client.Infrastructure.Routes.Festivals;

namespace HiSubmit.Client.Infrastructure.Managers.AdminFestival
{
    public interface IAdminFestivalManager : ITransientManager
    {
        Task<PaginatedResult<GetAllFestivalResponse>> GetAllAsync(GetAllFestivalRequest request);
        Task<IResult<int>> UpdateStateAsync(UpdateFestivalStateRequest request);
        Task<IResult> UpdateFeeStatus(UpdateFestivalFeeStatusRequest request);

        Task<IResult<GetFestivalDetailResponse>> GetFestivalDetailAsync
            (GetFestivalDetailByIdQuery query);
    }
    public class AdminFestivalManager : IAdminFestivalManager
    {
        private readonly HttpClient _httpClient;
        public AdminFestivalManager(HttpClient httpClient)
        {
            _httpClient=httpClient;
        }
        public async Task<PaginatedResult<GetAllFestivalResponse>> GetAllAsync(GetAllFestivalRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(AdminFestivalEndPoint.GetAll(),request);
            return await response.ToPaginatedResult<GetAllFestivalResponse>();
        }

        public async Task<IResult<int>> UpdateStateAsync(UpdateFestivalStateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(AdminFestivalEndPoint.UpdateState(), request);
            return await response.ToResult<int>();
        }

        public async Task<IResult> UpdateFeeStatus(UpdateFestivalFeeStatusRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync
                (AdminFestivalEndPoint.UpdateFeeStatus(), request);
            return await response.ToResult();
        }
        
        public async Task<IResult<GetFestivalDetailResponse>> GetFestivalDetailAsync
            (GetFestivalDetailByIdQuery query)
        {        
            var response = await _httpClient.GetAsync(AdminFestivalEndPoint.GetDetail(query));
            return await response.ToResult<GetFestivalDetailResponse>();
        }

    }
}
