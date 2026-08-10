namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetVrXrSpecificationDetail;

public class GetVrXrSpecificationDetailQuery 
{
    public int ProjectId { get; set; }
}

  
public class GetVrXrSpecificationDetailResponse
{
    public int Id { get; set; }
    public List<int> SubProjectTypeIds { get; set; }

    public string Genre { get; set; }
    public int RunTimeHours { get; set; }
    public int RunTimeMinutes { get; set; }
    public int RunTimeSecounds { get; set; }

    public bool VariableRunTime { get; set; }
    public string DescriptionRunTime { get; set; }
    public int MinRunTimeHours { get; set; }
    public int MinRunTimeMinutes { get; set; }
    public int MinRunTimeSecounds { get; set; }
    public int MaxTimeHours { get; set; }
    public int MaxTimeMinutes { get; set; }
    public int MaxTimeSecounds { get; set; }
    public int AvgTimeHours { get; set; }
    public int AvgTimeMinutes { get; set; }
    public int AvgTimeSecounds { get; set; }

    public DateTime CompletionDate { get; set; }
    public int ProductionBudget { get; set; }
    public int OriginCountryId { get; set; }
    public string OriginCountryName { get; set; }

    public string Language { get; set; }
    public bool StudentProject { get; set; }


    //navigationProperty
    public int ProjectId { get; set; }
    public GetVrXrSpecificationDetailResponse()
    {
        CompletionDate = DateTime.Today;
    }
}