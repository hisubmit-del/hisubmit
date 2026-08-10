namespace HiSubmit.Application.Models.Emails;

public class RefreeSubmitJudgingForProjectEmailViewModel:EmailViewModel
{
    public  int FestivalId { get; set; }
    public string RefereeFullName { get; set; }
    public  string ProjectTitle { get; set; }
    public  int ProjectJudgingId { get; set; }
}

public class EmailViewModel
{
    public  string Title { get; set; }
}