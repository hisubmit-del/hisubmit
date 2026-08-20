using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.Referee;

public interface ISelectionAdvisorManager : ITransientManager
{
    Task<IResult<List<SelectionRecommendationResponse>>> GetRecommendations(
        int festivalId,
        GetSelectionRecommendationsRequest request);
}

public class SelectionAdvisorManager(HttpClient httpClient) : ISelectionAdvisorManager
{
    private readonly BaseEndPoint _endpoint = new("api/v1/JudgingProject");

    public async Task<IResult<List<SelectionRecommendationResponse>>> GetRecommendations(
        int festivalId,
        GetSelectionRecommendationsRequest request)
    {
        var response = await httpClient.GetAsync(
            _endpoint.GenerateUrl($"{festivalId}/SelectionRecommendations", request));
        return await response.ToResult<List<SelectionRecommendationResponse>>();
    }
}
