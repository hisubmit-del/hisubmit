using HiSubmit.Client.Infrastructure.Constants;
using HiSubmit.Client.SharedModels.Constants.Application;

namespace Web.Handlers;

public class CookieHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var cookie = _httpContextAccessor.HttpContext?.Request?.Headers["Cookie"].ToString();
        if (!string.IsNullOrEmpty(cookie))
        {
            request.Headers.Add("Cookie", cookie);
        }
        //else
        //{
        //    var message = "";
        //    if (_httpContextAccessor != null)
        //    {
        //        if (_httpContextAccessor.HttpContext!=null)
        //        {
        //            if (_httpContextAccessor.HttpContext.Request != null)
        //            {
        //                if (_httpContextAccessor.HttpContext.Request.Cookies != null)
        //                    return base.SendAsync(request,cancellationToken);
        //                else
        //                {
        //                    message += "cookie null";
        //                }
        //            }
        //            else
        //            {
        //                message += "request null";
        //            }
        //        }
        //        else
        //        {
        //            message += "HTTPCONTEXT NULL";
        //        }
        //    }

        //    throw new Exception(message);
        //}
        return base.SendAsync(request, cancellationToken);
    }
}
