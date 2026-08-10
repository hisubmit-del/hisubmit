using Hisubmit.Client.SharedModels.Requests.AnswerQuestions;

namespace Hisubmit.Client.SharedModels.Features.Submits.Commands;

public class AddSubmitCommand 
{
    public int? ProjectId { get; set; }
    public int FestivalId { get; set; }
    public List<int> DeadlineEventCategoriesId { get; set; }
    public List<AnswerQuestionDto> SubmitAnswerQuestions { get; set; }
}
