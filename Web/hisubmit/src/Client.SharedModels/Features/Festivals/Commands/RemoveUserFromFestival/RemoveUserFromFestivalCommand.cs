namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.RemoveUserFromFestival;

public class RemoveUserFromFestivalCommand 
{
    public int? Id { get; set; }
    public int? FestivalId { get; set; }
    public string UserId { get; set; }
}

