using System.Net.Http;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Seo.GetPAgeSeoTags;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.Seo;

public interface ISeoManager:ITransientManager
{
    Task<IResult<GetPageSeoTagResult>> GetPageSeoTag(GetPageSeoTagsQuery query);
}

public class SeoManager(HttpClient httpClient) : ISeoManager
{
    private readonly BaseEndPoint _baseEndPoint = new("api/v1/public/seo");

    public async Task<IResult<GetPageSeoTagResult>> GetPageSeoTag(GetPageSeoTagsQuery query)
    {
        var res = await httpClient.GetAsync(_baseEndPoint.GenerateUrl("PageSeoTag", query));
        return await res.ToResult<GetPageSeoTagResult>();
    }
}