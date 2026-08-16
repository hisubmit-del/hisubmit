using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Commands.AddProjectJudgingResult;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests.AnswerQuestions;

namespace Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetDetail;

public class GetProjectJudgingDetailQuery
{
    public int Id { get; set; }
    public  int SubmitId { get; set; }
    public bool GetUserReferee { get; set; }
}

public class GetProjectJudgingDetailResponse
{
    public SubmitDto Submit { get; set; }
    public int SubmitId { get; set; }
    public string UserId { get; set; }    
    public int? JudgingButtonId { get; set; }
    public JudgingButtonDto JudgingButton { get; set; }
    public  string Comment { get; set; }

    public  string UserName { get; set; }
    public List<JudgingFieldAnswerDto> JudgingFiledAnswereds { get; set; }
    public List<AnswerQuestionDto> SubmitAnswerQuestions { get; set; }

    public GetProjectJudgingDetailResponse()
    {
        JudgingFiledAnswereds = new List<JudgingFieldAnswerDto>();
        SubmitAnswerQuestions = new List<AnswerQuestionDto>();
    }
}
public class JudgingButtonDto
{
    public int  Id { get; set; }
    public string Name { get; set; }
    public int JudgingId { get; set; }
}

public class SubmitDto
{
    public  int ProjectId { get; set; }
    public  string ProjectTitle { get; set; }
    public  ProjectType ProjectProjectType { get; set; }
    public  int FestivalId { get; set; }
    public DateTime SubmitDate { get; set; }
    public SubmitStatus SubmitStatus { get; set; }
    
   public string TrackingCode { get; set; }
    public string Comment { get; set; }
}
