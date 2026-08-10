using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditAdditinalSettings;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Client.SharedModels.Extensions;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;

public class GetFestivalDetailResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string UserId { get; set; }

    public string Description { get; set; }
    public string LogoURL { get; set; }
    public string Rules { get; set; }
    public int YearsRunning { get; set; }

    //Rewards
    public string RewardsName { get; set; }
    public string RewardLogoURL { get; set; }
    public string Rewards { get; set; }

    //Key Enable
    public int AudienceAttendence { get; set; }
    public int EstimatedSubmissions { get; set; }
    public int ProjectsSelected { get; set; }
    public int AwardsPresented { get; set; }


    //Event ItemType
    public bool FilmFestival { get; set; }
    public bool ScreenWritingWriter { get; set; }
    public bool MusicContest { get; set; }
    public bool PhotographicContest { get; set; }
    public bool OnlineFestival { get; set; }

    public string WebSite { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public AddEditAddressCommand Address { get; set; }

    //Social Media
    public string Facebook { get; set; }
    public string Twitter { get; set; }
    public string Instagram { get; set; }
    public string WhatsAppNumber { get; set; }
    public string Telegram { get; set; }
    public string Youtube { get; set; }

    //Submission Address
    public bool SeparateSubmissiionAddress { get; set; }
    public AddEditAddressCommand SubmissionAddress { get; set; }

    public bool OnlineEvent { get; set; }

    //DeadLine
    public DateTime? OpeningDate { get; set; }
    public DateTime? NotificationDate { get; set; }
    public DateTime? EventStartDate { get; set; }
    public DateTime? EventEndDate { get; set; }

    //Additional Settings
    public List<UpdateFestivalFestivalFocus> FestivalFestivalFoci { get; set; }
    public List<UpdateFestivalArtCategory> FestivalArtCategories { get; set; }
    public bool Public { get; set; }
    public string SearchTerms { get; set; }

    public bool AllLenghtAccepted { get; set; }
    public int? MinimomLenght { get; set; }
    public int? MaximomLenght { get; set; }
    public string URL { get; set; }

    //Tracking Sequence
    public int StartingNumber { get; set; }
    public string Prefix { get; set; }


    public  bool IsActive { get; set; }
    public bool ChangesNotAllowed => false;
    //FestivalStatus.UnderInvestigation == FestivalStatus ||
                                     //DateTime.Now.Between(EventStartDate, EventEndDate);

        
    public List<int> QualifyersId { get; set; }
        
        
        
    public  FestivalStatus FestivalStatus { get; set; }
        
    public  FeeStatus FeeStatus { get; set; }
    public string ApprovedLicenseURL { get; set; }

    public string GetTypeString()
    {
        var jj = new List<string>();
        if(OnlineFestival)jj.Add("Online FestivalId");
        if(FilmFestival)jj.Add("film festival");
        if(MusicContest)jj.Add("Music contest");
        if(ScreenWritingWriter)jj.Add("Screen Writing");
        if(PhotographicContest)jj.Add("Photographic contest");
        return string.Join(",", jj);
    }

    public string GetYearsRunning()
    {
        var s = YearsRunning switch
        {
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            > 3 => $"{YearsRunning}th",
            _ => ""
        };

        return s;
    }

    public string GetFestivalType()
    { 
        List<string> type = [];
        if(FilmFestival)
            type.Add("Film FestivalId");
        if(MusicContest)
            type.Add("Music Contest");
        if(ScreenWritingWriter)
            type.Add("Screen Writing");
        if(OnlineFestival)
            type.Add("Online FestivalId");
        if(PhotographicContest)
            type.Add("Photographic contest");

        return string.Join(',', type);
    }
}