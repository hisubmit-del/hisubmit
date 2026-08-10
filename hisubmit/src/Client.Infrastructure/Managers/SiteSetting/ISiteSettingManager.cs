using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Payments.Commands.EditSiteCommission;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.SiteSetting;

public interface ISiteSettingManager:ITransientManager
{
    Task<IResult> UpdateCommission(EditSiteCommissionCommand command);
    Task<IResult<GetSiteCommissionResponse>> GetSiteCommission();
}

public class SiteSettingManager : ISiteSettingManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public SiteSettingManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/admin/sitesetting");
    }

    public async Task<IResult> UpdateCommission(EditSiteCommissionCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("UpdateCommissions"), command);
        return await response.ToResult();
    }

    public async Task<IResult<GetSiteCommissionResponse>> GetSiteCommission()
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("Commissions"));
        return await response.ToResult<GetSiteCommissionResponse>();
    }
}