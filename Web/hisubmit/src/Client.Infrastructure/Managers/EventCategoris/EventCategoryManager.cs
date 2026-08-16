using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetEventCateoryById;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.EventCategoris
{
    public interface IEventCategoryManager : ITransientManager
    {
        Task<IResult<List<GetAllEventCategoryResponse>>> GetAllAsync(GetAllEventCategoryQuery query);
        Task<IResult<GetEventCategoryByIdResponse>> GetById(GetEventCategoryByIdQuery query);
        Task<IResult<int>> UpdateCategory(AddEditEventCategoryCommand commmand);
        Task<IResult<int>> DeleteCategory(DeleteEventCategoryCommand command);
    }
    public class EventCategoryManager : IEventCategoryManager
    {
        private HttpClient _httpClient;
        public EventCategoryManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<int>> DeleteCategory(DeleteEventCategoryCommand command)
        {
            var response = await _httpClient.DeleteAsync(Routes.Festivals.EventCategoryEndPoints.DeleteEventCategory(command));
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllEventCategoryResponse>>> GetAllAsync(GetAllEventCategoryQuery query)
        {
            var response =await _httpClient.GetAsync(Routes.Festivals.EventCategoryEndPoints.GetAllEventCategory(query));
            return await response.ToResult<List<GetAllEventCategoryResponse>>();
        }

        public async Task<IResult<GetEventCategoryByIdResponse>> GetById(GetEventCategoryByIdQuery query)
        {
            var response = await _httpClient.GetAsync(Routes.Festivals.EventCategoryEndPoints.GetEventCategoryById(query));
            return await response.ToResult<GetEventCategoryByIdResponse>();
        }

        public async Task<IResult<int>> UpdateCategory(AddEditEventCategoryCommand commmand)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.Festivals.EventCategoryEndPoints.UppdateEventCategory, commmand);
            return await response.ToResult<int>();
        }
    }
}
