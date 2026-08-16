using Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/{festivalId:int}")]
    [OriginAuthorize]
    public abstract class BaseFestivalController<T> : BaseApiController<T>
    {
    }
}