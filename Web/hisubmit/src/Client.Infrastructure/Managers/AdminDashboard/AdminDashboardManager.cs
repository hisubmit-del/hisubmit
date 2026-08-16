using System.Net.Http;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.Infrastructure.Extensions;
using Hisubmit.Hisubmit.Client.SharedModels.Features.AdminDashboard;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;

namespace HiSubmit.Client.Infrastructure.Managers.AdminDashboard;

public interface IAdminDashboardManager:ITransientManager
{
    Task<PaginatedResult<GetAllFestivalResponse>> GetAllUnderInvestigationFestivals();
    Task<IResult<GetFestivalAndUserStatusCount>> GetAccountStatusCount();
    Task<IResult<GetSitePurchaseResponse>> GetPurchase(GetSitePurchaseRequest request);
}
public class AdminDashboardManager
    (HttpClient httpClient) : IAdminDashboardManager
{
    private readonly BaseEndPoint _endPoint = new("api/v1/admin/adminDashboard");

    public async Task<PaginatedResult<GetAllFestivalResponse>> GetAllUnderInvestigationFestivals()
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("GetUnderInvestigationFestival"));
        return await response.ToPaginatedResult<GetAllFestivalResponse>();
    }

    public async Task<IResult<GetFestivalAndUserStatusCount>> GetAccountStatusCount()
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("GetAccountStatusCount"));
        return await response.ToResult<GetFestivalAndUserStatusCount>();
    }

    public async Task<IResult<GetSitePurchaseResponse>> GetPurchase(GetSitePurchaseRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("Purchase"));
        return await response.ToResult<GetSitePurchaseResponse>();
    }
}

