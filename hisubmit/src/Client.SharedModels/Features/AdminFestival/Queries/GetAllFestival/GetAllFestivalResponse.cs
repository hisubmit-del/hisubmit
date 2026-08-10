using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using System.ComponentModel.DataAnnotations;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditDeadLineEntry;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;

public record GetAllFestivalResponse
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public  string? Email { get; set; }
    public string? Name { get; set; }
    public string? LogoURL { get; set; }
    public bool FilmFestival { get; set; } = true;
    public bool ScreenWritingWriter { get; set; }
    public bool MusicContest { get; set; }
    public bool PhotographicContest { get; set; }
    public bool OnlineFestival { get; set; }
    public string? Phone { get; set; }
    public DateTime OpeningDate { get; set; }
    public DateTime? NotificationDate { get; set; }
    public DateTime? EventStartDate { get; set; }
    public DateTime? EventEndDate { get; set; }
    public string? URL { get; set; }
    public string? UserId { get; set; }
    public bool IsActive { get; set; }

    public int YearsRunning { get; set; }
    public AddEditAddressCommand Address { get; set; }

    public FestivalStatus FestivalStatus { get; set; }
        
    public  FeeStatus FeeStatus { get; set; }

    public string? Description { get; set; }

    public List<AddEditDeadLineEntryRequest> DeadLines { get; set; }

        
    public List<string?> Focuses { get; set; }

    public FestivalDateStatus FestivalDateStatus { get; set; }
    public string? DateTitle { get; set; }
    public DateTime? NearDeadline { get; set; }

    public double? MinFee { get; set; }
    public double? MaxFee { get; set; }

    public List<int> SelectedQualifiersId { get; set; } = new();
    public DateTime? CreatedOn { get; set; }
    public string AccountFullName { get; set; }
    public string AccountEmail { get; set; }

    public string GetFestivalType()
    {
        var type = string.Empty;
        var typeList = new List<string>();
        if (FilmFestival)
        {
            typeList.Add("Film FestivalId");
        }

        if (MusicContest)
        {
            typeList.Add("Music Contest");
        }

        if (PhotographicContest)
        {
            typeList.Add("Photographic Contest");
        }

        if (ScreenWritingWriter)
        {
            typeList.Add("Screen Writing");
        }

        return string.Join(",", typeList);
    }

    public int DescriptionCount()
    {
        return Description.Length < 200 ? Description.Length : 200;
    }
}

public enum FestivalDateStatus
{
    [Display(Name = "Open Soon")]
    OpenSoon,
    [Display(Name = "Submit")]
    Submit,
    [Display(Name = "Closed")]
    Closed,
}