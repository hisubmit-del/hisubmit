using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Domain.Enums;
using System;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Festivals;

public class Festival:AuditableEntity<int>
{
    public int FestivalMasterId { get; set; }
    public FestivalMaster FestivalMaster { get; set; }
    public string Name { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
    public string LogoURL { get; set; }      
    public string Rules { get; set; }
    public int YearsRunning { get; set; }

    //Reward
    public string RewardsName { get; set; }
    public string RewardLogoURL{ get; set; }
   
    public string Rewards { get; set; }


    //Key Enable
    public int AudienceAttendence { get; set; }
    public int EstimatedSubmissions { get; set; }
    public int ProjectsSelected { get; set; }
    public int AwardsPresented { get; set; }
    public List<EventOrginizer> EventOrginizers { get; set; }

    //EventType
    public ProjectType EventType { get; set; }
    public bool FilmFestival { get; set; } = true;
    public bool ScreenWritingWriter { get; set; }
    public bool MusicContest { get; set; }
    public bool PhotographicContest { get; set; }
    public bool OnlineFestival { get; set; }
    public bool ArtFestival { get; set; }
    //Contact
    public string WebSite { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public Address Address { get; set; }

    //Sociial Media
    public string Facebook { get; set; }
    public string Twitter { get; set; }
    public string Instagram { get; set; }
    public string WhatsAppNumber { get; set; }
    public string Telegram { get; set; }
    public string Youtube { get; set; }
    //Submission Address
    public bool SeparateSubmissiionAddress { get; set; }
    public Address SubmissionAddress { get; set; }

    //Venue
    public bool OnlineEvent { get; set; }
    public List<Venue> Venues { get; set; }

    //DeadLines
    public DateTime? OpeningDate { get; set; }
    public DateTime? NotificationDate { get; set; }
    public DateTime? EventStartDate { get; set; }
    public DateTime? EventEndDate { get; set; }
        

    public List<DeadLine> DeadLines { get; set; }
    //Additional Settings
    public List<FestivalFestivalFocus> FestivalFestivalFoci { get; set; }
    public List<FestivalArtCategory> FestivalArtCategories { get; set; }
    public bool Public { get; set; }
    public string SearchTerms { get; set; }
    public bool AllLenghtAccepted { get; set; }
    public int? MinimomLenght { get; set; }
    public int? MaximomLenght { get; set; }
    public string URL { get; set; }


    //Tracking Sequence
    public int StartingNumber { get; set; }
    public string Prefix { get; set; }

    public List<SubmissionQuestion> SubmissionQuestions { get; set; }
    public List<FestivalFestivalQualifying> FestivalFestivalQualifyings { get; set; }
    public List<FestivalFile> FestivalFiles { get; set; }
        

    
    
    public string ApprovedLicenseURL { get; set; }
    
    
    //Advanced
    public bool IsActive { get; set; }
    public  FestivalStatus FestivalStatus { get; set; }
    public List<FestivalSubUser> FestivalSubUsers { get; set; }
        
    //Event Categories
    public  List<EventCategory> EventCategories { get; set; }
        
        
    //image galleries
    public List<Image> Images { get; set; }
       
    //Fee Enable
    public  FeeStatus FeeStatus { get; set; }
        
    //backGround job Id
    public  string SendNotificationDateEmailJobId { get; set; }
    public  string SendOpenDateEmailJobId { get; set; }
    public  string SendEventStartDateEmailJobId { get; set; }
    public  string SendEventEndDateEmailJobId { get; set; }
    
    
    //Normalize for Search
    public double? MinFee { get; set; }
    public double? MaxFee { get; set; }
    public bool IsActivePeriod { get; set; }
    public bool EnableAutomaticPeriodCreation { get; set; }
    public bool EnableAutomaticSelectionNews { get; set; } = true;
    
    public Festival Clone()
    {
        return (Festival)
            MemberwiseClone();
    }
}
