using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;

public class GetAllSubmitsResponse
{
    public int Id { get; set; }

    public int FestivalId { get; set; }
    public string FestivalName { get; set; }
    public  string FestivalLogoUrl { get; set; }

    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; }
    public string ProjectFileURl { get; set; }
    public string ProjectOwnerId { get; set; }
    public string ProjectOwnerFullName { get; set; }
    public string ProjectUrl { get; set; }
    public DateTime SubmitDate { get; set; }
    public SubmitStatus SubmitStatus { get; set; }
    public JudgingStatus JudgingStatus { get; set; }

    public List<DeadLineCategoryDto> DeadlineEventCategories { get; set; }
    public string ProjectEnglishBriefSynopsis { get; set; }
    public ProjectType ProjectProjectType { get; set; }
    
    
    public string TrackingCode { get; set; }

    public GetAllSubmitsResponse()
    {
        DeadlineEventCategories = new List<DeadLineCategoryDto>();
    }
}

public class DeadLineCategoryDto
{
    public int Id { get; set; }
    public int GoldFee { get; set; }
    public int StudentFee { get; set; }
    public int StandardFee { get; set; }
    public int EventCategoryId { get; set; }
    public int DeadLineId { get; set; }
    public string EventCategoryName { get; set; }
    public string DeadLineName { get; set; }
}