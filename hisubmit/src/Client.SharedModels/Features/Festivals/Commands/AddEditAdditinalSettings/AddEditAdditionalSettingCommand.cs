using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditAdditinalSettings;

public class AddEditAdditionalSettingCommand 
{
    public int Id { get; set; }
    public List<int> FestivalFestivalFociId { get; set; } = new();
    public List<int> FestivalArtCategoriesId { get; set; } = new();
    public bool Public { get; set; }
    public string SearchTerms { get; set; }

    public bool AllLenghtAccepted { get; set; }
    public int? MinimomLenght { get; set; }
    public int? MaximomLenght { get; set; }
    public string URL { get; set; }

    //Tracking Sequence
    public int StartingNumber { get; set; }
    public string Prefix { get; set; }
        
    public  FestivalStatus FestivalStatus { get; set; }
        
    public bool ChangesNotAllowed { get; set; }
    public bool EnableAutomaticPeriodCreation { get; set; }
    public bool EnableAutomaticSelectionNews { get; set; } = true;
}
