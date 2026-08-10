namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddFestival;

public class AddFestivalCommand
{
    public string UserId { get; set; }
    public  string Name { get; set; }
    public bool AddToCurrentUser { get; set; }
}


