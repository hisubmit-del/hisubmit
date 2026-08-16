using Hisubmit.Client.SharedModels.Features.Submits.Commands;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Reviews.Commands;
using Hisubmit.Client.SharedModels.Features.Reviews.Queries;

namespace HiSubmit.Client.Infrastructure.Managers.Submits;

public class SubmitManager : ISubmitManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;
    public SubmitManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/ProjectSubmitted");
    }
    

    public async Task<IResult> FinalResult(AddEditFinalJudgingCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("FinalResult"), command);
        return await response.ToResult<int>();
    }

    public async Task<IResult> WithDraw(WithDrawProjectCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("WithDraw"),command);
        return await response.ToResult();
    }

    public async Task<PaginatedResult<GetAllSubmitsResponse>> GetAll(GetAllSubmitsRequest request)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("GetAll",request));
        return await response.ToPaginatedResult<GetAllSubmitsResponse>();
    }

    public async Task<IResult<int>> SubmitToFestival(AddSubmitCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("Submit"), command);
        return await response.ToResult<int>();
    }

    public async Task<IResult> Review(AddReviewCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("Review"), command);
        return await response.ToResult();
    }
        
    public  async  Task<PaginatedResult<GetAllReviewResponse>> GetAllReview(GetAllReviewQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("AllReview",query));
        return await response.ToPaginatedResult<GetAllReviewResponse>();
    }
}