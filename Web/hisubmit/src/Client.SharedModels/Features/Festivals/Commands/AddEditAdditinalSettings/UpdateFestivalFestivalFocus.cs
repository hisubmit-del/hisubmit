namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditAdditinalSettings;

public class UpdateFestivalFestivalFocus
{
    public int Id { get; set; }
    public int FestivalId { get; set; }
    public int FestivalFocusId { get; set; }
    public object FestivalFocusName { get; set; }
}