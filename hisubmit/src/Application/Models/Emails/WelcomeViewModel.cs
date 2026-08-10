namespace HiSubmit.Application.Models.Emails;

public class WelcomeViewModel
{
    public  string FullName { get; set; }
}


public class ConfirmedEmailModel
{
    public string FullName { get; set; }
    public string VerificationCode { get; set; }
}