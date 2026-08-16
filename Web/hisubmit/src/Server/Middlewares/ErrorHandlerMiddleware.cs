using HiSubmit.Application.Exceptions;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using System.Linq;

namespace HiSubmit.Server.Middlewares
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
                string baseAddress;

                _logger.LogError(addressFeature.Addresses.First());


                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = await Result<string>.FailAsync(error.Message);
                _logger.LogError("------------------------------------------------------------------------------------------\n"
                                 + error.StackTrace
                                 + "\n/////////////////////////////////////////////////////////////////////////////////// \n"
                                 + error.Message
                                 + "\n/////////////////////////////////////////////////////////////////////////////////// \n"
                                 + error.InnerException
                                 + "\n------------------------------------------------------------------------------------------ \n");
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