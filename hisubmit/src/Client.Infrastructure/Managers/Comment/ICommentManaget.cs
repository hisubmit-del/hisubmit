using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using Hisubmit.Client.SharedModels.Features.Comments.Commands;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.Comment;

public interface ICommentManager:ITransientManager
{
    Task<IResult> AddCommentAsync(AddCommentCommand comment);
}

public class CommentManager(HttpClient httpClient) : ICommentManager
{
    BaseEndPoint endPoint=new BaseEndPoint("api/v1/Comment");
    public async Task<IResult> AddCommentAsync(AddCommentCommand comment)
    {
        var response = await httpClient.PostAsJsonAsync(endPoint.GenerateUrl("AddComment"),comment);
        var data = await response.ToResult();
        return data;
    }
}