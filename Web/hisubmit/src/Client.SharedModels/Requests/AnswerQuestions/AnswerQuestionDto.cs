namespace Hisubmit.Client.SharedModels.Requests.AnswerQuestions;

public class AnswerQuestionDto
{

    public int Id { get; set; }
    public int SubmissionQuestionId { get; set; }

    //public int? JudgingId { get; set; }
    //public int? FestivalId { get; set; }

    public string Answer { get; set; }
}