using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalDeadlines;

public class AddEditFestivalDeadlineCommand
{
    public int Id { get; set; }
    public DateTime? OpeningDate { get; set; }
    public DateTime? NotificationDate { get; set; }
    public DateTime? EventStartDate { get; set; }
    public DateTime? EventEndDate { get; set; } 
    public  FestivalStatus FestivalStatus { get; set; }
    public bool ChangesNotAllowed { get; set; }
    public AddEditFestivalDeadlineCommand()
    {
        OpeningDate = DateTime.Now;
    }
}