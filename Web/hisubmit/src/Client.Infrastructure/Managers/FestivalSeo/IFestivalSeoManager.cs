using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Features.Seo.GetPAgeSeoTags;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Managers.AdminSeo;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.FestivalSeo;

public interface IFestivalSeoManager:ITransientManager
{
    Task<IResult> AddEditSeoTags(AddEditSeoTagRequest request);
    Task<IResult<GetPageSeoTagResult>> GetPageSeoTag(GetPageSeoTagsQuery query);
}

public class FestivalSeoManager(HttpClient httpClient) : IFestivalSeoManager
{
    private readonly BaseEndPoint _endPoint = new("api/v1/seo");

    public async Task<IResult> AddEditSeoTags(AddEditSeoTagRequest request)
    {
        var res = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl($"{request.PageId}/SeoTagsSetting"), request);
        return await res.ToResult();
    }
    
    public async Task<IResult<GetPageSeoTagResult>> GetPageSeoTag(GetPageSeoTagsQuery query)
    {
        var res = await httpClient.GetAsync(_endPoint.GenerateUrl($"{query.PageId}/SeoTags"));
        return await res.ToResult<GetPageSeoTagResult>();
    }
}
