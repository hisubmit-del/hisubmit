using HiSubmit.Server.Filters;
using Microsoft.AspNetCore.Mvc;

namespace HiSubmit.Server.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/{festivalId:int}")]
    [OriginAuthorize]
    public abstract class BaseFestivalController<T> : BaseApiController<T>
    {
    }
}