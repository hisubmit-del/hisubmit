using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace Hisubmit.Client.SharedModels.Requests.Identity
{
    public class RegisterRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        private string email;

        [Required]
        [EmailAddress]
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                UserName = value;
            }
        }

        [Required]
        [MinLength(6)]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string FestivalName { get; set; }

        public int? FestivalId { get; set; }

        public bool RegisterAsFestival { get; set; }
        public bool ActivateUser { get; set; }
        public bool AutoConfirmEmail { get; set; }
    }
}