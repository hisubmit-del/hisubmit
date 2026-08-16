using HiSubmit.Client.SharedModels.Constants.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/v{version:apiVersion}/admin/[controller]")]
    [Authorize(Roles =RoleConstants.AdministratorRole)]
    public abstract class BaseAdminController<T> : BaseApiController<T>
    {
        
    }
}

