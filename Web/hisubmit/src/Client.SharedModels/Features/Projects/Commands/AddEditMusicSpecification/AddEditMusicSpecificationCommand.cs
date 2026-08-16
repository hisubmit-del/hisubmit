namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditMusicSpecification;

public class AddEditMusicSpecificationCommand 
{
    public int Id { get; set; }
    public List<int> SubProjectTypeIds { get; set; }

    public string Genre { get; set; }
    public int RunTimeHours { get; set; }
    public int RunTimeMinutes { get; set; }
    public int RunTimeSecounds { get; set; }
    public DateTime? CompletionDate { get; set; }
    public int OriginCountryId { get; set; }

    public string Language { get; set; }

    public bool StudentProject { get; set; }

    //navigation Property
    public int ProjectId { get; set; }
    public AddEditMusicSpecificationCommand()
    {
        SubProjectTypeIds = new List<int>();
        CompletionDate = DateTime.Today;
    }
}