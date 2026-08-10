using Hisubmit.Client.SharedModels.Features.Judgings.Commands.AddEditJudgiingButton;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.AddEditJudgingButton;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.DeleteJudgiingFiiled;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.DeleteJudgingButtons;
using Hisubmit.Client.SharedModels.Features.Judgings.Queries.Detail;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Judgings
{
    public class JudgingManager : IJudgingManager
    {
        private HttpClient _httpClient;
            public JudgingManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IResult<int>> AddButton(AddEditJudgingButtonCommand command, int festivalId)
        {
            var response = await _httpClient.PostAsJsonAsync(JudgingEndpoint.AddEditButton(festivalId),command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> AddFiled(AddEditJudgingFiledCommand command, int festivalId)
        {
            var response = await _httpClient.PostAsJsonAsync(JudgingEndpoint.AddEditFiled(festivalId), command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> DeleteButton(DeleteJudgingButtonCommand command, int festivalId)
        {
            var response = await _httpClient.DeleteAsync(JudgingEndpoint.DeleteButton(command,festivalId));
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> DeleteFiled(DeleteJudgingFiledCommand command, int festivalId)
        {
            var response = await _httpClient.DeleteAsync(JudgingEndpoint.DeleteFiled(command, festivalId));
            return await response.ToResult<int>();
        }

        public async Task<IResult<GetJudgingDetailResponse>> GetDetail(GetJudgingDetailQuery query)
        {
            var response = await _httpClient.GetAsync(JudgingEndpoint.Detail(query));
            return await response.ToResult<GetJudgingDetailResponse>();
        }
    }
}
