using System.Net.Http;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;
using Hisubmit.Hisubmit.Client.SharedModels.Features.MasterFestivals.Queries;

namespace HiSubmit.Client.Infrastructure.Managers.MasterFestivals;

public interface IMasterFestivalManager:ITransientManager
{
    Task<PaginatedResult<GetAllMasterFestivalResponse>> GetAll(GetAllMasterFestivalRequest request);
}
public class MasterFestivalManager(HttpClient httpClient) : IMasterFestivalManager
{
    private readonly BaseEndPoint _endPoint = new BaseEndPoint("api/v1/admin/festivalMaster");

    public async Task<PaginatedResult<GetAllMasterFestivalResponse>> GetAll(GetAllMasterFestivalRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("getAll", request));
        return await response.ToPaginatedResult<GetAllMasterFestivalResponse>();
    }
}
