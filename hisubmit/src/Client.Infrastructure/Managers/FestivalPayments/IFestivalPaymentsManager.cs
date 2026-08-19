using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using System.Collections.Generic;
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
using Hisubmit.Client.SharedModels.Features.Settlements.Commands;
using Hisubmit.Client.SharedModels.Features.Settlements.Queries;

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

    Task<IResult<List<FestivalSettlementStatementResponse>>> GetSettlementStatements(
        GetFestivalSettlementStatementsQuery query);
    Task<IResult> CreateSettlementStatement(CreateFestivalSettlementStatementCommand command);
    Task<IResult> AddSettlementAdjustment(int festivalId, int statementId,
        AddSettlementAdjustmentCommand command);
    Task<IResult> UpdateSettlementStatus(int festivalId, int statementId,
        UpdateSettlementStatusCommand command);
    Task<byte[]> ExportSettlementStatement(int festivalId, int statementId, string format);
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

    public async Task<IResult<List<FestivalSettlementStatementResponse>>> GetSettlementStatements(
        GetFestivalSettlementStatementsQuery query)
    {
        var response = await _httpClient.GetAsync(
            _baseEndPoint.GenerateUrl($"{query.FestivalId}/SettlementStatements", query));
        return await response.ToResult<List<FestivalSettlementStatementResponse>>();
    }

    public async Task<IResult> CreateSettlementStatement(
        CreateFestivalSettlementStatementCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(
            _baseEndPoint.GenerateUrl($"{command.FestivalId}/SettlementStatements"), command);
        return await response.ToResult();
    }

    public async Task<IResult> AddSettlementAdjustment(
        int festivalId, int statementId, AddSettlementAdjustmentCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(
            _baseEndPoint.GenerateUrl($"{festivalId}/SettlementStatements/{statementId}/adjustments"),
            command);
        return await response.ToResult();
    }

    public async Task<IResult> UpdateSettlementStatus(
        int festivalId, int statementId, UpdateSettlementStatusCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(
            _baseEndPoint.GenerateUrl($"{festivalId}/SettlementStatements/{statementId}/status"),
            command);
        return await response.ToResult();
    }

    public async Task<byte[]> ExportSettlementStatement(
        int festivalId, int statementId, string format)
    {
        var response = await _httpClient.GetAsync(
            _baseEndPoint.GenerateUrl(
                $"{festivalId}/SettlementStatements/{statementId}/export",
                new ExportFestivalSettlementQuery { FestivalId = festivalId, StatementId = statementId, Format = format }));
        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsByteArrayAsync()
            : [];
    }
}
