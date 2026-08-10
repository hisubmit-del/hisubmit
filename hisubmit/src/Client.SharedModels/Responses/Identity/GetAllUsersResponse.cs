using System.Collections.Generic;

namespace Hisubmit.Client.SharedModels.Responses.Identity
{
    public class GetAllUsersResponse
    {
        public IEnumerable<UserResponse> Users { get; set; }
    }
}