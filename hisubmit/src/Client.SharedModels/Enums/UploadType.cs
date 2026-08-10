using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums;
public enum UploadType : byte
{
    [Description(@"Images\Products")]
    Product = 0,

    [Description(@"Images\ProfilePictures")]
    ProfilePicture = 1,

    [Description(@"Documents")]
    Document = 2,

    [Description(@"Advertise")]
    Advertise = 3,

    [Description(@"Festival\Organizer")]
    Organizer = 4,

    [Description(@"Festivals\Logo")]
    FestivalLogo = 5,

    [Description(@"Festivals\reward")]
    FestivalRewardFile = 6,

    [Description(@"FestivalId\file")]
    FestivalFile = 7,

    [Description(@"Project\file")]
    ProjectFile = 8,

    [Description(@"Project\universityCard")]
    UniversityCard = 9,

    [Description(@"FestivalId\image\")]
    FestivalImage = 10,

    [Description(@"News\banner")]
    NewBanner = 11,

    [Description(@"FestivalId\ApprovedLicense")]
    ApprovedLicense = 12,
    [Description(@"Project\Awards")]
    Awards = 13,
    [Description(@"Project\Credit")]
    Credit = 14
}

public enum UserType : byte
{
    Base = 1,
    Gold = 2
}


public enum ShowInSiteStatus
{
    [Display(Name = "Pending admin approval")]
    WaitForApproved,
    [Display(Name = "Enable")]
    Enable,
    [Display(Name = "Disable")]
    Disable,
}