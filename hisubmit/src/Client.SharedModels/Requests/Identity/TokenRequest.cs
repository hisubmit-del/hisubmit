using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Requests.Identity;

public class TokenRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}


public class VerificationCodeRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
}

public class ResendVerificationCodeRequest
{
    public string Email { get; set; }
}
