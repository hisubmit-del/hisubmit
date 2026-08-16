using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}