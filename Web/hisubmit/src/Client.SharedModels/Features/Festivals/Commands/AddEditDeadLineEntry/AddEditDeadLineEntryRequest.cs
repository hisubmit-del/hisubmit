namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditDeadLineEntry;

public class AddEditDeadLineEntryRequest 
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int FestivalId { get; set; }
    public DateTime? Date { get; set; } = DateTime.Today;
    public List<int> CategoryId { get; set; } = new();
    public bool ApplyToAllCategory { get; set; }

    public bool AddWithoutCategory { get; set; }
}
