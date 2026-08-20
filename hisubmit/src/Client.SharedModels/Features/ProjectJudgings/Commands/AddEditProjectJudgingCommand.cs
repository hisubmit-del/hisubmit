namespace Hisubmit.Client.SharedModels.Features.ProjectJudgings.Commands;

public record AddEditProjectJudgingCommand
    (List<int> SubmitsId, List<string> UsersId, int FestivalId, bool AssignToReferee) 
   
{
    public bool MultiProjectToMultiReferee { get; set; }
    public List<int> DeadlineEventCategoryIds { get; set; } = [];
}
