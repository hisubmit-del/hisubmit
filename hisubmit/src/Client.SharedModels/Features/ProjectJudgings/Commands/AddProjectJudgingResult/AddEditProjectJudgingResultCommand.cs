

using Hisubmit.Client.SharedModels.Requests.AnswerQuestions;

namespace Hisubmit.Client.SharedModels.Features.ProjectJudgings.Commands.AddProjectJudgingResult;

public class AddEditProjectJudgingResultCommand 
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public int? JudgingButtonId { get; set; }

    public List<AnswerQuestionDto> SubmitAnswerQuestions { get; set; }
    public List<JudgingFieldAnswerDto> JudgingFiledAnswers { get; set; }
}

public class JudgingFieldAnswerDto
{
    public int Id { get; set; }
    public int Rate { get; set; }
    public int JudgingFiledId { get; set; }
}