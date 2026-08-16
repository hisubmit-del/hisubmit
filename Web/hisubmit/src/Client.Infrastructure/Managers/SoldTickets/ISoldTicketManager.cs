using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Commands;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.SoldTickets;

public interface ISoldTicketManager:ITransientManager
{
    Task<IResult> AddBadgeToCart(AddSoldBadgeCommand command);
    Task<IResult> AddTicketToCart(AddSoldTicketCommand command);
    Task<PaginatedResult<GetAllSoldTicketResponse>> GetAllSoldTicket(GetAllSoldTicketQuery query);
    Task<IResult<DownloadTicketFileResponse>> DownloadTickets(DownloadTicketsFileQuery query);
}


public class SoldTicketManager : ISoldTicketManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _baseEndPoint;

    public SoldTicketManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseEndPoint = new BaseEndPoint("api/v1/soldTicket");
    }
    public async Task<IResult> AddBadgeToCart(AddSoldBadgeCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_baseEndPoint.GenerateUrl("AddBadgeToCart"), command);
        return await response.ToResult();
    }

    public async Task<IResult> AddTicketToCart(AddSoldTicketCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_baseEndPoint.GenerateUrl("AddTicketToCart"), command);
        return await response.ToResult();
    }

    public async Task<PaginatedResult<GetAllSoldTicketResponse>> GetAllSoldTicket(GetAllSoldTicketQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl("GetAll", query));
        return await response.ToPaginatedResult<GetAllSoldTicketResponse>();
    }

    public async Task<IResult<DownloadTicketFileResponse>> DownloadTickets(DownloadTicketsFileQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl("Download", query));
        return await response.ToResult<DownloadTicketFileResponse>();
    }
}