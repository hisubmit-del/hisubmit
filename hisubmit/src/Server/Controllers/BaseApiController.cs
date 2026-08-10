using HiSubmit.Server.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HiSubmit.Server.Controllers;

/// <summary>
/// Abstract BaseApi Controller Class
/// </summary>
[ApiController]
[OriginAuthorize]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController<T> : ControllerBase
{
    private IMediator _mediatorInstance;
    private ILogger<T> _loggerInstance;
    protected IMediator Mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();
    protected ILogger<T> Logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
}

[ApiController]
[OriginAuthorize]
[Route("api/v{version:apiVersion}/public/[controller]")]
public abstract class BasePublicController<T> : ControllerBase
{
    private IMediator _mediatorInstance;
    private ILogger<T> _loggerInstance;
    protected IMediator Mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();
    protected ILogger<T> Logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
}
