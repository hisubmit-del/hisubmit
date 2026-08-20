using Hisubmit.Client.SharedModels.Features.DistributionInformations.Commands;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditAward;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectCreditCommand;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectFileURL;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.DeleteProjectFiles;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.EditProjectSubmitterInformation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UpdateScreenWritings;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UploadProjectFile;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllDistribuationInformationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectCredits;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAwardDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetProjectFileDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetProjectSpecifications;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetScreenAward;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.Infrastructure.Routes.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Permissions.Queries;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.ProjectImages;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.ReleaseProject;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UpdateProjectFileOrder;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectImages;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllSubProjectType;
using Hisubmit.Client.SharedModels.Features.SubProjectTypes.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Recommendations.Queries;

namespace HiSubmit.Client.Infrastructure.Managers.Projects
{
    public interface IProjectManager : ITransientManager
    {
        Task<IResult<int>> UpdateDetailAsync(AddEditProjectDetailCommand command);
        Task<IResult<GetProjectDetailResponse>> GetDetailAsync(GetProjectDetailQuery query);
        Task<PaginatedResult<GetAllProjectResponse>> GetAllAsync(GetAllProjectRequest request);
        Task<IResult<int>> UpdateSubmitterAsync(EditProjectSubmitterInformationCommand command);
        Task<IResult<int>> UpdateCredit(UpdateProjectCreditsRequest request);
        Task<IResult<List<GetAllProjectCreditResponse>>> GetAllProjectCreditAsync(GetAllProjectCreditQuery query);
        Task<IResult> UpdateScreenAwards(UpdateScreenWritingRequest request);
        Task<IResult<List<GetScreenAwardResponse>>> DetailScreenAward(GetScreenAwardRequest request);
        Task<IResult> UpdateAwards(UpdateAwardRequest request);
        Task<IResult<List<GetAwardDetailResponse>>> DetailAward(GetAwardDetailRequest request);
        Task<IResult> UpdateDistributionInformation(UpdateDistributionInformationCommand command);

        Task<IResult<List<AddEditDistributionInformationRequest>>> DetailDistributionInformation(
            GetAllDistribuationInformationQuery query);

        Task<IResult<AddEditFileUrlResponse>> UpdateProjectFileURL(AddEditProjectFileURLRequest request);
        Task<IResult> UploadProjectFile(int projectId, int fregment, MultipartFormDataContent content);
        Task<IResult<List<GetAllProjectFileResponse>>> GetAllFiles(GetAllProjectFilesQuery query);
        Task<IResult<GetProjectFileDetailResponse>> GetProjectFileDetail(GetProjectFileDetailQuery query);
        Task<IResult> DeleteProjectFile(DeleteProjectFilesCommand command);
        Task<IResult<GetProjectSpecificationResponse>> GetProjectSpecification(GetProjectSpecificationQuery query);
        Task<IResult> ReleaseProject(ReleaseProjectCommand command);

        Task<IResult<List<GetAllSubProjectTypeResponse>>> GetAllSelectedSubProjectType(
            GetAllSubProjectSelectedTypeQuery query);

        Task<IResult> AddProjectImage(AddProjectImageCommand command);
        Task<PaginatedResult<GetAllProjectImageResponse>> GetAllProjectImage(GetAllProjectImagesQuery query);
        Task<IResult> UpdateProjectFileOrders(UpdateProjectFileOrderCommand command);
        Task<IResult<List<GoldFestivalRecommendation>>> GetGoldFestivalRecommendations(
            GetGoldFestivalRecommendationsRequest query);
    }

    public class ProjectManager : IProjectManager
    {
        private readonly HttpClient _httpClient;
        private readonly BaseEndPoint _projectEndPoint;

        public ProjectManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _projectEndPoint = new BaseEndPoint("api/v1/project");
        }

        public async Task<IResult<List<GoldFestivalRecommendation>>> GetGoldFestivalRecommendations(
            GetGoldFestivalRecommendationsRequest query)
        {
            var response = await _httpClient.GetAsync(
                ProjectEndPoint.GoldFestivalRecommendations(query));
            return await response.ToResult<List<GoldFestivalRecommendation>>();
        }

        public async Task<IResult<List<GetAwardDetailResponse>>> DetailAward(GetAwardDetailRequest request)
        {
            var response = await _httpClient.GetAsync(ProjectEndPoint.AllAwards(request));
            return await response.ToResult<List<GetAwardDetailResponse>>();
        }

        public async Task<IResult<List<GetScreenAwardResponse>>> DetailScreenAward(GetScreenAwardRequest request)
        {
            var response = await _httpClient.GetAsync(ProjectEndPoint.AllScreenAwards(request));
            return await response.ToResult<List<GetScreenAwardResponse>>();
        }

        public async Task<IResult<List<AddEditDistributionInformationRequest>>> DetailDistributionInformation(
            GetAllDistribuationInformationQuery query)
        {
            var response = await _httpClient.GetAsync(ProjectEndPoint.AllDistribuation(query));
            return await response.ToResult<List<AddEditDistributionInformationRequest>>();
        }

        public async Task<PaginatedResult<GetAllProjectResponse>> GetAllAsync(GetAllProjectRequest request)
        {
            var response = await _httpClient.GetAsync(ProjectEndPoint.GetAll(request));
            return await response.ToPaginatedResult<GetAllProjectResponse>();
        }

        public async Task<IResult<List<GetAllProjectCreditResponse>>> GetAllProjectCreditAsync(
            GetAllProjectCreditQuery query)
        {
            var response = await _httpClient.GetAsync(ProjectEndPoint.AllCredits(query));
            return await response.ToResult<List<GetAllProjectCreditResponse>>();
        }

        public async Task<IResult<GetProjectDetailResponse>> GetDetailAsync(GetProjectDetailQuery query)
        {
            var response = await _httpClient.GetAsync(ProjectEndPoint.GetDetail(query));
            return await response.ToResult<GetProjectDetailResponse>();
        }

        public async Task<IResult> UpdateAwards(UpdateAwardRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ProjectEndPoint.UpdateAward(), request);
            return await response.ToResult();
        }

        public async Task<IResult<int>> UpdateCredit(UpdateProjectCreditsRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(ProjectEndPoint.UpdateCredit(), request);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> UpdateDetailAsync(AddEditProjectDetailCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(ProjectEndPoint.UpdateDetail(), command);
            return await response.ToResult<int>();
        }

        public async Task<IResult> UpdateDistributionInformation(UpdateDistributionInformationCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(ProjectEndPoint.UpdateDistribuation(), command);
            return await response.ToResult();
        }

        public async Task<IResult> UpdateScreenAwards(UpdateScreenWritingRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ProjectEndPoint.UpdateScreenAward(), request);
            return await response.ToResult();
        }

        public async Task<IResult<int>> UpdateSubmitterAsync(EditProjectSubmitterInformationCommand command)
        {
            var response = await _httpClient.PutAsJsonAsync(ProjectEndPoint.UpdateSubmmiiter(), command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<AddEditFileUrlResponse>> UpdateProjectFileURL(AddEditProjectFileURLRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(_projectEndPoint.GenerateUrl("UpdateFileURL"), request);
            return await response.ToResult<AddEditFileUrlResponse>();
        }

        public async Task<IResult> UploadProjectFile(int projectId, int fregment, MultipartFormDataContent content)
        {
            var response = await _httpClient.PostAsync(
                $"{_projectEndPoint.GenerateUrl("UploadFile")}?projectId={projectId}&fregment={fregment}", content);
            return await response.ToResult();
        }

        public async Task<IResult<List<GetAllProjectFileResponse>>> GetAllFiles(GetAllProjectFilesQuery query)
        {
            var response = await _httpClient.GetAsync($"{_projectEndPoint.GenerateUrl("AllFiles", query)}");
            return await response.ToResult<List<GetAllProjectFileResponse>>();
        }

        public async Task<IResult<GetProjectFileDetailResponse>> GetProjectFileDetail(GetProjectFileDetailQuery query)
        {
            var response = await _httpClient.GetAsync($"{_projectEndPoint.GenerateUrl("ProjectFileDetail", query)}");
            return await response.ToResult<GetProjectFileDetailResponse>();
        }

        public async Task<IResult> DeleteProjectFile(DeleteProjectFilesCommand command)
        {
            var response =
                await _httpClient.DeleteAsync($"{_projectEndPoint.GenerateUrl("DeleteProjectFile", command)}");
            return await response.ToResult();
        }

        public async Task<IResult<GetProjectSpecificationResponse>> GetProjectSpecification(
            GetProjectSpecificationQuery query)
        {
            var response = await _httpClient.GetAsync($"{_projectEndPoint.GenerateUrl("GetSpecification", query)}");
            return await response.ToResult<GetProjectSpecificationResponse>();
        }

        public async Task<IResult> ReleaseProject(ReleaseProjectCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_projectEndPoint.GenerateUrl("Release")}", command);
            return await response.ToResult();
        }

        public async Task<IResult<List<GetAllSubProjectTypeResponse>>> GetAllSelectedSubProjectType(
            GetAllSubProjectSelectedTypeQuery query)
        {
            var response =
                await _httpClient.GetAsync($"{_projectEndPoint.GenerateUrl("GetSelectedSpecification", query)}");
            return await response.ToResult<List<GetAllSubProjectTypeResponse>>();
        }

        public async Task<IResult> AddProjectImage(AddProjectImageCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(_projectEndPoint.GenerateUrl("AddProjectImage"), command);
            return await response.ToResult();
        }

        public async Task<PaginatedResult<GetAllProjectImageResponse>> GetAllProjectImage(
            GetAllProjectImagesQuery query)
        {
            var response = await _httpClient.GetAsync($"{_projectEndPoint.GenerateUrl("GetAllProjectImage", query)}");
            return await response.ToPaginatedResult<GetAllProjectImageResponse>();
        }

        public async Task<IResult> UpdateProjectFileOrders(UpdateProjectFileOrderCommand command)
        {
            var response = await _httpClient.PostAsJsonAsync(_projectEndPoint.GenerateUrl("UpdateProjectFileOrder"), command);
            return await response.ToResult();
        }
    }
}
