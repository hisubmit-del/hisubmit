using Hisubmit.Client.SharedModels.Enums;
using System.ComponentModel.DataAnnotations;
using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.CreateFestival;

public class AddEditFestivalDetailCommand 
{
    public int Id { get; set; }
    [Required] public string Name { get; set; }
    public int YearsRunning { get; set; }
    public List<EventType> EventTypes { get; set; }
    public string Description { get; set; }
    public string LogoURL { get; set; }
    public string Rewards { get; set; }
    public string RewardsName { get; set; }
    public string RewardLogoURL { get; set; }
    public string Rules { get; set; }
    public int AudienceAttendence { get; set; }
    public int EstimatedSubmissions { get; set; }
    public int ProjectsSelected { get; set; }
    public int AwardsPresented { get; set; }

    public bool FilmFestival { get; set; }
    public bool ScreenWritingWriter { get; set; }
    public bool MusicContest { get; set; }
    public bool PhotographicContest { get; set; }
    public bool OnlineFestival { get; set; }
    public bool ArtFestival { get; set; }

    public List<string> QualifyersId { get; set; }

    public UploadRequest UploadRequest { get; set; }
    public UploadRequest RewardLogoUploadRequest { get; set; }

    public FestivalStatus FestivalStatus { get; set; }

    public bool ChangesNotAllowed { get; set; }

    public UploadRequest ApprovedLicenseUploadRequest { get; set; }
        
    public string ApprovedLicenseURL { get; set; }

    public AddEditFestivalDetailCommand()
    {
        QualifyersId = new List<string>();
    }
}