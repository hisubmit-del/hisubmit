using System;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetAll;

public class GetAllProjectJudgingResponse
{
    public int Id { get; set; }
    public int SubmitId { get; set; }
    public string UserId { get; set; }
    public int ProjectId { get; set; }
    public int FestivalId { get; set; }
    public string ProjectURL { get; set; }
    public string ProjectName { get; set; }
    public DateTime CreatedOn { get; set; }
    public string FestivalName { get; set; }
    public  string UserFullName { get; set; }
    public  string ProjectOwner { get; set; }

    public int? JudgingButtonId { get; set; }
    public string JudgingButtonName { get; set; }
    public double JudgingFiledAverage { get; set; }
        
    public RefereeStatus RefereeStatus { get; set; }
    public string ProjectFileUrl { get; set; }
}