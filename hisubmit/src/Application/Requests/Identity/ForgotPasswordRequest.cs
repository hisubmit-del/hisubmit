using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Application.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}