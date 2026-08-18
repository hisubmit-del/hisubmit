using System.ComponentModel.DataAnnotations;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;

public class GetAllFestivalRequest : PagedRequest
{
    public string? SearchString { get; set; }
    public DateTime? OpeningDateFrom { get; set; }
    public DateTime? OpeningDateTo { get; set; }
    public string? Name { get; set; }
    public bool OpenOnly { get; set; }
    public bool TicketOnly { get; set; }
    public bool? IsActive { get; set; }
    public FeeStatus? FeeStatus { get; set; }
    public bool? FilmFestival { get; set; }
    public bool? ScreenWritingWriter { get; set; }
    public bool? MusicContest { get; set; }
    public bool? PhotographicContest { get; set; }
    public bool? OnlineFestival { get; set; }

    public List<int> Categories { get; set; } = new();
    public int? Category { get; set; } 

    //public string CategoriesIdString { get; set; }
    public List<int> Focuses { get; set; } = new();
    
    public int? Focus { get; set; }

    // public string FocusIdString { get; set; }
    public FestivalStatus? FestivalStatus { get; set; }
    public int CountryId { get; set; }

    public int Runtime { get; set; }

    public RangeType FeeRangeType { get; set; }
    public double FeeSecond { get; set; }
    public double FeeFirst { get; set; }

    public RangeType YearsRunningRangeType { get; set; }
    public int YearsRunningSecond { get; set; }
    public int YearsRunningFirst { get; set; }

    public RangeType EntryDeadlineRangeType { get; set; }
    public DateTime? EntryDeadlineFrom { get; set; }
    public DateTime? EntryDeadlineTo { get; set; }
    public DateTime? EventDateTo { get; set; }
    
    public bool PublicOnly { get; set; }
    public FestivalType? FestivalType { get; set; }

    public List<int> SelectedQualifiersId { get; set; } = new();
    
    public int? FeeMinVal { get; set; }
    public int? FeeMaxVal { get; set; }
    
    public int? YearsRunningMinVal { get; set; }
    public int? YearsRunningMaxVal { get; set; }

    public GetAllFestivalRequest()
    {
        Orderby = new[] { ""};
    }
}

public enum RangeType
{
    [Display(Name = "Not Selected")] NotSelected,
    [Display(Name = "Equal")] Equal,
    [Display(Name = "Greater than")] After,
    [Display(Name = "Less than")] Before,
    [Display(Name = "Between")] Between
}

public enum FestivalType
{
    [Display(Name = "Film Festival")]
    FilmFestival,
    [Display(Name = "Screen Writing Writer")]
    ScreenWritingWriter,
    [Display(Name = "Music Contest")]
    MusicContest,
    [Display(Name = "Photographic Contest")]
    PhotographicContest,
    [Display(Name = "Online Festival")]
    OnlineFestival
}
