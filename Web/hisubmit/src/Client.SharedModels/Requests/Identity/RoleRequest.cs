using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Requests.Identity
{
    public class RoleRequest
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int? FestivalId { get; set; }
    }
}