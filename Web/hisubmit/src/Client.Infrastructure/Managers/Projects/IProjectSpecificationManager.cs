using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditFilmSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditMusicSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditPhotographySpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditScriptSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditVrXrSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetMusicSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetVrXrSpecificationDetail;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Projects
{
    public interface IProjectSpecificationManager:ITransientManager
    {
        Task<IResult<int>> UpdateFilmSpecification(AddEditFilmSpecificationCommand command);
        Task<IResult<GetFilmSpecificationDetailResponse>> GetFilmSpecification(GetFilmSpecificationDetailRequest request);

        Task<IResult<int>> UpdateMusicSpecification(AddEditMusicSpecificationCommand command);
        Task<IResult<GetMusicSpecificationDetailResponse>> GetMusicSpecification(GetMusicSpecificationDetailQuery query);

        Task<IResult<int>> UpdateScriptSpecification(AddEditScriptSpecificationCommand command);
        Task<IResult<GetScriptSpecificationDetailResponse>> GetScriptSpecification(GetScriptSpecificationDetailQuery query);

        Task<IResult<int>> UpdatePhotographySpecification(AddEditPhotographySpecificationCommand command);
        Task<IResult<GetPhotographySpecificationDetailResponse>> GetPhotographySpecification(GetPhotographySpecificationDetailQuery query);

        Task<IResult<int>> UpdateXrVrSpecification(AddEditVrXrSpecificationCommand command);
        Task<IResult<GetVrXrSpecificationDetailResponse>> GetXrVRSpecification(GetVrXrSpecificationDetailQuery query);


    }

    public class ProjectSpecificationManager : IProjectSpecificationManager
    {
        private HttpClient _httpClient;
        public ProjectSpecificationManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<GetFilmSpecificationDetailResponse>> GetFilmSpecification(GetFilmSpecificationDetailRequest request)
        {
            var response = await _httpClient.GetAsync(ProjectSpecification.GetFilmSpecification(request));
            return await response.ToResult<GetFilmSpecificationDetailResponse>();
        }

        public async Task<IResult<GetScriptSpecificationDetailResponse>> GetScriptSpecification(GetScriptSpecificationDetailQuery query)
        {
            var response = await _httpClient.GetAsync(ProjectSpecification.GetScriptSpecification(query));
            return await response.ToResult<GetScriptSpecificationDetailResponse>();
        }

        public async Task<IResult<GetMusicSpecificationDetailResponse>> GetMusicSpecification(GetMusicSpecificationDetailQuery query)
        {
            var response = await _httpClient.GetAsync(ProjectSpecification.GetMusicSpecification(query));
            return await response.ToResult<GetMusicSpecificationDetailResponse>();
        }

        public async Task<IResult<GetPhotographySpecificationDetailResponse>> GetPhotographySpecification(GetPhotographySpecificationDetailQuery query)
        {
            var response = await _httpClient.GetAsync(ProjectSpecification.GetPhotographySpecification(query));
            return await response.ToResult<GetPhotographySpecificationDetailResponse>();
        }

        public async Task<IResult<GetVrXrSpecificationDetailResponse>> GetXrVRSpecification(GetVrXrSpecificationDetailQuery query)
        {
            var response = await _httpClient.GetAsync(ProjectSpecification.GetVrXrSpecification(query));
            return await response.ToResult<GetVrXrSpecificationDetailResponse>();
        }

        public async Task<IResult<int>> UpdateFilmSpecification(AddEditFilmSpecificationCommand command)
        {
            var response = await _httpClient.PutAsJsonAsync(ProjectSpecification.UpdateFilmSpecification(), command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> UpdateMusicSpecification(AddEditMusicSpecificationCommand command)
        {
            var response = await _httpClient.PutAsJsonAsync(ProjectSpecification.UpdateMusicSpecification(), command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> UpdatePhotographySpecification(AddEditPhotographySpecificationCommand command)
        {
            var response = await _httpClient.PutAsJsonAsync(ProjectSpecification.UpdatePhotographySpecification(), command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> UpdateScriptSpecification(AddEditScriptSpecificationCommand command)
        {
            var response = await _httpClient.PutAsJsonAsync(ProjectSpecification.UpdateScriptSpecification(), command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<int>> UpdateXrVrSpecification(AddEditVrXrSpecificationCommand command)
        {
            var response = await _httpClient.PutAsJsonAsync(ProjectSpecification.UpdateVrXrSpecification(), command);
            return await response.ToResult<int>();
        }
    }
}
