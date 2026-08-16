using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitCategories;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetSubmitDetail;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetSubmitFormAnswers;
using Hisubmit.Client.SharedModels.Requests.AnswerQuestions;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.FestivalSubmit;

public interface IFestivalSubmitManager : ITransientManager
{
    Task<PaginatedResult<GetAllSubmitsResponse>> GetAllSubmitAsync(GetAllSubmitsRequest request);

    Task<PaginatedResult<GetAllSubmitCategoriesResponse>> GetAllSubmitCategoriesAsync(GetAllSubmitCategoriesQuery query);

    Task<IResult<List<AnswerQuestionDto>>> GetSubmitFormAnswers(GetSubmitFormAnswersQuery query);

    Task<IResult<GetAllSubmitsResponse>> GetSubmitDetailAsync(GetSubmitDetailQuery query);
}

public class FestivalSubmitManager : IFestivalSubmitManager
{
    private readonly BaseEndPoint _baseEndPoint;
    private readonly HttpClient _httpClient;

    public FestivalSubmitManager(HttpClient httpClient)
    {
        _baseEndPoint = new BaseEndPoint("api/v1/festivalSubmits");
        _httpClient = httpClient;
    }
    public async Task<PaginatedResult<GetAllSubmitsResponse>> GetAllSubmitAsync(GetAllSubmitsRequest request)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl($"{request.FestivalId}/submits", request));
        return await response.ToPaginatedResult<GetAllSubmitsResponse>();
    }

    public async Task<PaginatedResult<GetAllSubmitCategoriesResponse>> GetAllSubmitCategoriesAsync(GetAllSubmitCategoriesQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl($"{query.FestivalId}/submitCategories", query));
        return await response.ToPaginatedResult<GetAllSubmitCategoriesResponse>();
    }

    public async Task<IResult<List<AnswerQuestionDto>>> GetSubmitFormAnswers(GetSubmitFormAnswersQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl($"{query.FestivalId}/submitFormAnswers", query));
        return await response.ToResult<List<AnswerQuestionDto>>();
    }

    public async Task<IResult<GetAllSubmitsResponse>> GetSubmitDetailAsync(GetSubmitDetailQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl($"{query.FestivalId}/SubmitDetail", query));
        return await response.ToResult<GetAllSubmitsResponse>();
    }
}