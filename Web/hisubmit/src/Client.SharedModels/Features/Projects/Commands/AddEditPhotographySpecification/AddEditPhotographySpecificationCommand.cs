namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditPhotographySpecification;

public class AddEditPhotographySpecificationCommand 
{
    public int Id { get; set; }
    public string Genre { get; set; }
    public DateTime? TakenDate { get; set; }
    public int OriginCountryId { get; set; }
    public string Camera { get; set; }
    public string Lens { get; set; }
    public string FocalLength { get; set; }
    public string ShutterSpeed { get; set; }
    public string Aperture { get; set; }
    public string Iso_Film { get; set; }
    public string Location { get; set; }
    public bool StudentProject { get; set; }

    public List<int> SubProjectTypeIds { get; set; }

    //navigation propety
    public int ProjectId { get; set; }

    public AddEditPhotographySpecificationCommand()
    {
        SubProjectTypeIds = new List<int>();
        TakenDate = DateTime.Now;
    }
}