using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Domain.Enums;
using System;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Projects;

public class Project : AuditableEntity<int>
{
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public ProjectType ProjectType { get; set; }
    public bool HasNoneEnglishTitle { get; set; }
    public string EnglishBriefSynopsis { get; set; }
    public string OriginalTitle { get; set; }
    public string OriginalBriefSynopsis { get; set; }
    public string WebSite { get; set; }
    public string Twitter { get; set; }
    public string Youtube { get; set; }
    public string Instagram { get; set; }
    public string Telegram { get; set; }
    public string WhatsApp { get; set; }

    public int Size { get; set; }

    //Credit
    public List<ProjectCredit> ProjectCredits { get; set; }


    //specification
    public FilmSpecification FilmSpecification { get; set; }

    public ScriptSpecification ScriptSpecification { get; set; }

    public MusicSpecification MusicSpecification { get; set; }

    public PhotographySpecification PhotographySpecification { get; set; }

    public XrVrSpecification XrVrSpecification { get; set; }

    //Submitter

    public bool UseCurrentUserInformation { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Address Address { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }

    //awards and distribution
    public List<Award> Awards { get; set; }
    public List<ScreeningAward> ScreeningAwards { get; set; }
    public List<DistributionInformation> DistributionInformation { get; set; }

    //File
    public bool IsLocalFile { get; set; }
    public string FileURl { get; set; }
    public string LocalFileURL { get; set; }
    public string Password { get; set; }
    public string FileDescription { get; set; }
    
    public string URL { get; set; }
    
    public bool StudentProject { get; set; }
    public string UniversityName { get; set; }
    public string StudentPhotoCard { get; set; }

    public List<ProjectFile> ProjectFiles { get; set; }
    public List<ProjectImage> ProjectImages { get; set; }

    public ProjectStatus ProjectStatus { get; set; }
}