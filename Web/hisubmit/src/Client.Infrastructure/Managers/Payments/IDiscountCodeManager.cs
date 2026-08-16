using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

namespace HiSubmit.Client.Infrastructure.Managers.Payments;

public interface IDiscountCodeManager:ITransientManager
{
    Task<IResult> AddEdit(AddEditDiscountCodeRequest request);
    Task<IResult> Delete(BaseDeleteRequest request,int festivalId);
    Task<PaginatedResult<GetAllDiscountCodeResponse>> GetAllDiscountCode(DiscountCodeFilter filter);
    Task<IResult> ChangeStatus(ChangeDiscountCodeStatusRequest request);
}


public class DiscountCodeManager (HttpClient httpClient)
    : IDiscountCodeManager
{
    private readonly BaseEndPoint _endPoint=new("/api/v1/discountCode");
    public async Task<IResult> AddEdit(AddEditDiscountCodeRequest request)
    {
        var res = await httpClient.PostAsJsonAsync
            (_endPoint.GenerateUrl($"{request.FestivalId}/addEdit"), request);
        return await res.ToResult();
    }

    public async Task<IResult> Delete(BaseDeleteRequest request,int festivalId)
    {
        var res = await httpClient.DeleteAsync(_endPoint.GenerateUrl($"{festivalId}/delete", request));
        return await res.ToResult();
    }

    public async Task<PaginatedResult<GetAllDiscountCodeResponse>> GetAllDiscountCode(DiscountCodeFilter filter)
    {
        var res = await httpClient.PostAsJsonAsync
            (_endPoint.GenerateUrl($"{filter.FestivalId}/GetAll"), filter);
        return await res.ToPaginatedResult<GetAllDiscountCodeResponse>();
    }

    public async Task<IResult> ChangeStatus(ChangeDiscountCodeStatusRequest request)
    {
        var res = await httpClient.PostAsJsonAsync
            (_endPoint.GenerateUrl($"{request.FestivalId}/changeStatus"), request);
        return await res.ToPaginatedResult<GetAllDiscountCodeResponse>();
    }
}