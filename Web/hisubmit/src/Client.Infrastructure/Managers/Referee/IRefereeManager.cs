using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Commands.AddProjectJudgingResult;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.CheckPermissionForJudging;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetDetail;
using Hisubmit.Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetRefereeData;

namespace HiSubmit.Client.Infrastructure.Managers.Referee
{
    public interface IRefereeManager:ITransientManager
    {
        Task<PaginatedResult<GetAllProjectJudgingResponse>> GetAllAsync(GetAllProjectJudgingQuery query);
        Task<IResult<CheckPermissionResponse>> CheckPermission(string projectUrl);
        Task<IResult<int?>> AddRefereeResult(AddEditProjectJudgingResultCommand command);
        Task<IResult<GetProjectJudgingDetailResponse>> GetProjectJudgingDetail(GetProjectJudgingDetailQuery query);
        Task<IResult<GetRefereeDataResponse>> GetRefereeData(GetRefereeDataRequest request);
    }

    public class RefereeManager(HttpClient httpClient) : IRefereeManager
    {
        private readonly BaseEndPoint _endPoint = new("api/v1/Referee");

        public async Task<PaginatedResult<GetAllProjectJudgingResponse>> GetAllAsync(GetAllProjectJudgingQuery query)
        {
            var response =await httpClient.GetAsync(_endPoint.GenerateUrl("GetAll",query));
            return await response.ToPaginatedResult<GetAllProjectJudgingResponse>();
        }

        public async Task<IResult<CheckPermissionResponse>> CheckPermission(string projectUrl)
        {
            var response = await httpClient.GetAsync(_endPoint.GenerateUrl($"checkPermission/{projectUrl}"));
            return await response.ToResult<CheckPermissionResponse>();
        }

        public async Task<IResult<int?>> AddRefereeResult(AddEditProjectJudgingResultCommand command)
        {
            var response = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("AddJudgment"), command);
            return await response.ToResult<int?>();
        }

        public async Task<IResult<GetProjectJudgingDetailResponse>> GetProjectJudgingDetail(GetProjectJudgingDetailQuery query)
        {
            var response = await httpClient.GetAsync(_endPoint.GenerateUrl("DetailJudging", query));
            return await response.ToResult<GetProjectJudgingDetailResponse>();
        }

        public async Task<IResult<GetRefereeDataResponse>> GetRefereeData(GetRefereeDataRequest request)
        {
            var response = await httpClient.GetAsync(_endPoint.GenerateUrl("GetUserRefereeData", request));
            return await response.ToResult<GetRefereeDataResponse>();
        }
    }
}

