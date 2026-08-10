namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditShowHall;

public class AddEditShowHallCommand 
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Capacity { get; set; }
    public int AvailableCapacity { get; set; }

    public int VenueId { get; set; }

    public List<ShowTimeDto> ShowTimes { get; set; }

    public AddEditShowHallCommand()
    {
        ShowTimes = new List<ShowTimeDto>();
    }
}

public class ShowTimeDto
{
    public int Id { get; set; }
    public  string Name { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }

    public  int AvailableCapacity { get; set; }

    public int ShowHallId { get; set; }


    public override string ToString()
    {
        return $"{Name}:{OpenDate?.Date} {OpenDate?.TimeOfDay}- {OpenDate?.TimeOfDay}";
    }
}
