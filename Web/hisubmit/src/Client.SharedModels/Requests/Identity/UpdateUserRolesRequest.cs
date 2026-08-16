using Hisubmit.Client.SharedModels.Responses.Identity;

namespace Hisubmit.Client.SharedModels.Requests.Identity
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }
    }
}


