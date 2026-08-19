using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Extensions;
using HiSubmit.Application.Exceptions;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Net;
using System.Text.Json;

namespace Web.Middlewares
{
    public class ErrorHandlerMiddleware
    {


        private readonly RequestDelegate _next;
        private ILogger<ErrorHandlerMiddleware> _logger;
        private readonly IServer _server;
        public ErrorHandlerMiddleware(RequestDelegate next,
            ILogger<ErrorHandlerMiddleware> logger, IServer server)
        {
            _next = next;
            _server = server;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(correlationId) || correlationId.Length > 100)
                correlationId = Guid.NewGuid().ToString("N");
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var addressFeature = _server.Features.Get<IServerAddressesFeature>();
                var requestUrl = context.Request.GetDisplayUrl();
                var serverAddress = addressFeature?.Addresses.FirstOrDefault();

                _logger.LogError(
                    error,
                    "Unhandled request error. Method: {Method}, Path: {Path}, RequestUrl: {RequestUrl}, ServerAddress: {ServerAddress}",
                    context.Request.Method,
                    context.Request.Path,
                    requestUrl,
                    serverAddress ?? "unknown"
                );

                // A streaming Blazor response may already have started when a
                // component fails during prerendering. Headers and status code
                // cannot be changed after that point; logging is the only safe
                // recovery action.
                if (context.Response.HasStarted)
                {
                    return;
                }

                var response = context.Response;
                response.ContentType = "application/json";
                var responseMessage = error is ApiException or KeyNotFoundException
                    ? error.Message
                    : "An unexpected error occurred while processing your request.";
                var responseModel = await Result<string>.FailAsync(responseMessage);
                responseModel.Messages.Add($"Correlation ID: {correlationId}");

                switch (error)
                {
                    case ApiException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;

                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    //case BadRequestException e:
                    //    response.StatusCode = (int)HttpStatusCode.Conflict;
                    //    break;

                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
