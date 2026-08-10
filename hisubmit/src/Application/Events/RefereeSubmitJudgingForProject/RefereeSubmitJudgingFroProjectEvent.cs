using MediatR;

namespace HiSubmit.Application.Events.RefereeSubmitJudgingForProject;

public class RefereeSubmitJudgingFroProjectEvent:INotification
{
    public int SubmitId { get; set; }
    public int FestivalId { get; set; }
    public  int ProjectJudgingId { get; set; }
}