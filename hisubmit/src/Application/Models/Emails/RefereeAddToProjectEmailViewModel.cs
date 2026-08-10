namespace HiSubmit.Application.Models.Emails;

public class RefereeAddToProjectEmailViewModel
{
    public  string ProjectTitle { get; set; }
    public  int ProjectJudgingId { get; set; }
    public  string Email { get; set; }
    public int FestivalId { get; set; }
    public  string Title { get; set; }
    public string FestivalName { get; set; }
}