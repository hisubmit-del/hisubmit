using Hisubmit.Client.SharedModels.Features.SubProjectTypes.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Catalogs;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllSubProjectType;

namespace HiSubmit.Client.Infrastructure.Managers.SubProjectTypes
{
    public interface ISubProjectTypeManager:ITransientManager
    {
        Task<IResult<List<GetAllSubProjectTypeResponse>>> GetAllAsync(GetAllSubProjectTypeQuery query);
    }

    public class SubProjectTypeManager : ISubProjectTypeManager
    {
        private HttpClient _httpClient;
        public SubProjectTypeManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<List<GetAllSubProjectTypeResponse>>> GetAllAsync(GetAllSubProjectTypeQuery query)
        {
            var response =await _httpClient.GetAsync(SubProjectTypEndPoints.GetAll(query));
            return await response.ToResult<List<GetAllSubProjectTypeResponse>>();
        }
    }
}
