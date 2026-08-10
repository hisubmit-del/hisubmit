using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Commands;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.CheckPermissionForJudging;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.JudgingProjects
{
    public interface IProjectJudgingManager:ITransientManager
    {
        Task<IResult<int>> AddJudging(AddEditProjectJudgingCommand command);
        Task<PaginatedResult<GetAllProjectJudgingResponse>> GetAll(GetAllProjectJudgingQuery query);
    }
    public class ProjectJudgingManager : IProjectJudgingManager
    {
        private HttpClient _httpClient;
        private readonly BaseEndPoint _endPoint;

        public ProjectJudgingManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _endPoint = new BaseEndPoint("api/v1/JudgingProject");
        }

        public async Task<IResult<int>> AddJudging(AddEditProjectJudgingCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl($"{0}/AddRefree"),command);
            return await response.ToResult<int>();
        }

        public async Task<PaginatedResult<GetAllProjectJudgingResponse>> GetAll(GetAllProjectJudgingQuery query)
        {
            var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{0}/ProjecJudgings", query));
            return await response.ToPaginatedResult<GetAllProjectJudgingResponse>();
        }
    }
}

