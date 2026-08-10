using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalFileDetail;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.FestivalFiles
{
    public class FestivalFileManager : IFestivalFileManager
    {
        private readonly HttpClient _httpClient;
        public FestivalFileManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IResult<int>> DeleteAsync(DeleteFestivalFileCommand command, int festivalId)
        {
            var response = await _httpClient.DeleteAsync(FestivalFileEndPoint.Delete(command,festivalId));
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllFestivalFileResponse>>> GetAllAsync(GetAllFestivalFileQuery query, int festivalId)
        {
            var response = await _httpClient.GetAsync(FestivalFileEndPoint.GetAll(query,festivalId));
            return await response.ToResult<List<GetAllFestivalFileResponse>>();
        }

        public async Task<IResult<GetFestivalFileDetailResponse>> GetDetailAsync(GetFestivalFileDetailQuery query, int festivalId)
        {
            var response = await _httpClient.GetAsync(FestivalFileEndPoint.GetDetail(query,festivalId));
            return await response.ToResult<GetFestivalFileDetailResponse>();
        }

        public async Task<IResult<int>> UpdateAsync(AddEditFestivalFileCommand commmand, int festivalId)
        {
            var response = await _httpClient.PostAsJsonAsync(FestivalFileEndPoint.Update(festivalId),commmand);
            return await response.ToResult<int>();
        }
      
    }
}
