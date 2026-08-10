using System.Net.Http;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.SpecialAccounts.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace  HiSubmit.Client.Infrastructure.Managers.UsersAccounts;

public interface IUserAccountManager : ITransientManager
{
    Task<IResult<GetUserAccountTypeResponse>> GetAccountType(GetUserAccountTypeQuery query);
}

public class UserAccountManager(HttpClient httpClient) : IUserAccountManager
{
    private readonly BaseEndPoint _endPoint = new("api/v1/userAccount");

    public async Task<IResult<GetUserAccountTypeResponse>> GetAccountType(GetUserAccountTypeQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("AccountStatus", query));
        return await response.ToResult<GetUserAccountTypeResponse>();
    }
}

