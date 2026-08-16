using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Submits.Commands;

public class AddEditFinalJudgingCommand
{
    public List<int> SubmitId { get; set; }
    public string Comment { get; set; }
    public  JudgingStatus JudgingStatus { get; set; }
    public  SubmitStatus SubmitStatus { get; set; }
}