using System.Collections.Generic;

namespace Hisubmit.Client.SharedModels.Responses.Identity
{
    public class GetAllRolesResponse
    {
        public IEnumerable<RoleResponse> Roles { get; set; }
    }
}