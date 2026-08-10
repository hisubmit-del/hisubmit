using System.Net.Http;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.Email;

public interface IEmailManager:ITransientManager
{
    Task<IResult> Send();
}
public  class  EmailManager:IEmailManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _baseEndPoint;

    public EmailManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseEndPoint = new BaseEndPoint("api/v1/email");
    }
    public async Task<IResult> Send()
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl("SendEmail"));
        return await response.ToResult();
    }
}