using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Queries.FestivalPaymentStates;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Queries.GetDetail;

namespace HiSubmit.Client.Infrastructure.Managers.FestivalPayments;

public interface IFestivalPaymentsManager : ITransientManager
{
    Task<PaginatedResult<GetCartItemResponse>> GetAll(GetAllCartItemQuery query);

    Task<IResult<GetFestivalPaymentInformationDetailResponse>> GetFestivalPaymentInformationAsync(
        GetFestivalPaymentInformationDetailQuery query);

    Task<IResult> UpdateFestivalPaymentInformation(AddEditFestivalPaymentInformationCommand command);

    Task<PaginatedResult<GetAllFestivalPaymentItemResponse>> GetAllFestivalPaymentItem(
        GetAllFestivalPaymentItemQuery query);

    Task<IResult<GetFestivalPaymentStateResponse>> GetFestivalPaymentState
        (GetFestivalPaymentStateQuery query);
}

public class FestivalPaymentsManager : IFestivalPaymentsManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _baseEndPoint;

    public FestivalPaymentsManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseEndPoint = new BaseEndPoint("api/v1/FestivalPayments");
    }

    public async Task<PaginatedResult<GetCartItemResponse>> GetAll(GetAllCartItemQuery query)
    {
        var response = await _httpClient.GetAsync
            (_baseEndPoint.GenerateUrl($"{query.FestivalId}/CartItems", query));
        return await response.ToPaginatedResult<GetCartItemResponse>();
    }

    public async Task<IResult<GetFestivalPaymentInformationDetailResponse>>
        GetFestivalPaymentInformationAsync(GetFestivalPaymentInformationDetailQuery query)
    {
        var response =
            await _httpClient.GetAsync(_baseEndPoint.GenerateUrl($"{query.FestivalId}/GetPaymentInformation", query));
        return await response.ToResult<GetFestivalPaymentInformationDetailResponse>();
    }

    public async Task<IResult> UpdateFestivalPaymentInformation(AddEditFestivalPaymentInformationCommand command)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                _baseEndPoint.GenerateUrl($"{command.FestivalId}/UpdatePaymentInformation"), command);
        return await response.ToResult();
    }
    
    public async Task<PaginatedResult<GetAllFestivalPaymentItemResponse>> GetAllFestivalPaymentItem(GetAllFestivalPaymentItemQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl($"{query.FestivalId}/AllFestivalPaymentItems", query));
        return await response.ToPaginatedResult<GetAllFestivalPaymentItemResponse>();
    }
    
    public async Task<IResult<GetFestivalPaymentStateResponse>> GetFestivalPaymentState(GetFestivalPaymentStateQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl($"{query.FestivalId}/DemandFestival", query));
        return await response.ToResult<GetFestivalPaymentStateResponse>();
    }
}