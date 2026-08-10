using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Commands.Add;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Queries.FestivalPaymentStates;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using Hisubmit.Client.SharedModels.Features.Payments.Commands;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.AdminPaymentManager;

public interface IAdminPaymentManager:ITransientManager
{
    Task<PaginatedResult<GetCartItemResponse>> GetAllCartItemAsync(PaymentFilterDto query);
    Task<PaginatedResult<GetAllCartsResponse>> GetAllCartAsync(GetAllCartsFilterDto  query);

    Task<PaginatedResult<GetAllFestivalPaymentInformationResponse>> GetAllFestivalPaymentInformationAsync(
        GetAllFestivalPaymentInformationQuery query);

    Task<IResult<GetFestivalPaymentInformationDetailResponse>> GetFestivalPaymentInformationPaymentAsync(
        GetFestivalPaymentInformationDetailQuery query);

    Task<PaginatedResult<GetAllFestivalPaymentItemResponse>> GetAllFestivalPaymentItem(
        GetAllFestivalPaymentItemQuery query);

    Task<IResult> AddFestivalPaymentItem(AddFestivalPaymentItemCommand command);

    Task<IResult<GetFestivalPaymentStateResponse>> GetFestivalPaymentState(GetFestivalPaymentStateQuery query);

    Task<IResult<DownloadCartFactorResponse>> DownloadFactor(DownloadCartFactorRequest request);
}

public class AdminPaymentManager : IAdminPaymentManager
{
    private readonly BaseEndPoint _endPoint;
    private readonly HttpClient _httpClient;

    public AdminPaymentManager(HttpClient httpClient)
    {
        _endPoint = new BaseEndPoint("api/v1/admin/payment");
        _httpClient = httpClient;
    }
    public async Task<PaginatedResult<GetCartItemResponse>> GetAllCartItemAsync(PaymentFilterDto query)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("Items"),query);
        return await response.ToPaginatedResult<GetCartItemResponse>();
    }

    public async Task<PaginatedResult<GetAllCartsResponse>> GetAllCartAsync(GetAllCartsFilterDto query)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("Carts"),query);
        return await response.ToPaginatedResult<GetAllCartsResponse>();
    }

    public async Task<PaginatedResult<GetAllFestivalPaymentInformationResponse>> GetAllFestivalPaymentInformationAsync(GetAllFestivalPaymentInformationQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("AllPaymentsInformation",query));
        return await response.ToPaginatedResult<GetAllFestivalPaymentInformationResponse>();
    }
    
    public async Task<IResult<GetFestivalPaymentInformationDetailResponse>> GetFestivalPaymentInformationPaymentAsync(GetFestivalPaymentInformationDetailQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("GetPaymentInformation",query));
        return await response.ToResult<GetFestivalPaymentInformationDetailResponse>();
    }

    public async Task<PaginatedResult<GetAllFestivalPaymentItemResponse>> GetAllFestivalPaymentItem(GetAllFestivalPaymentItemQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("AllFestivalPaymentItems", query));
        return await response.ToPaginatedResult<GetAllFestivalPaymentItemResponse>();
    }

    public async Task<IResult> AddFestivalPaymentItem(AddFestivalPaymentItemCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("AddFestivalPaymentItem"),command);
        return await response.ToResult();
    }
    
        
    public async Task<IResult<GetFestivalPaymentStateResponse>> GetFestivalPaymentState(GetFestivalPaymentStateQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("DemandFestival", query));
        return await response.ToResult<GetFestivalPaymentStateResponse>();
    }

    public async Task<IResult<DownloadCartFactorResponse>> DownloadFactor(DownloadCartFactorRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("DownloadCartFactor"),request);
        return await response.ToResult<DownloadCartFactorResponse>();
    }
}