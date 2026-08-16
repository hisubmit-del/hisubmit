namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditScriptSpecification;

public class AddEditScriptSpecificationCommand
{
    public int Id { get; set; }
    public List<int> SubProjectTypeIds { get; set; }
    public string Genre { get; set; }
    public int NumberOfPage { get; set; }
    public int OriginCountryId { get; set; }
    public string Language { get; set; }
    public bool StudentProject { get; set; }
    public bool FirstTimeScreenWrite { get; set; }

    //navigation property
    public int ProjectId { get; set; }

    public AddEditScriptSpecificationCommand()
    {
        SubProjectTypeIds = new List<int>();
    }
}