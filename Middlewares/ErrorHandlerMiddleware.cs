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
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var addressFeature = _server.Features.Get<IServerAddressesFeature>();
                var requestUrl = context.Request.GetDisplayUrl();
                var serverAddress = addressFeature?.Addresses.FirstOrDefault();

                var response = context.Response;
                response.ContentType = "application/json";
                var responseMessage = error is ApiException or KeyNotFoundException
                    ? error.Message
                    : "An unexpected error occurred while processing your request.";
                var responseModel = await Result<string>.FailAsync(responseMessage);

                _logger.LogError(
                    error,
                    "Unhandled request error. Method: {Method}, Path: {Path}, RequestUrl: {RequestUrl}, ServerAddress: {ServerAddress}",
                    context.Request.Method,
                    context.Request.Path,
                    requestUrl,
                    serverAddress ?? "unknown"
                );

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