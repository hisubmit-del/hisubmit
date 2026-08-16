using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditFilmSpecification;

public class AddEditFilmSpecificationCommand 
{
    public int Id { get; set; }
    public List<int> SubProjectTypeIds { get; set; }
    public string Genre { get; set; }
    public int RunTimeHours { get; set; }
    public int RunTimeMinutes { get; set; }
    public int RunTimeSecounds { get; set; }
    public DateTime? CompletionDate { get; set; }
    public int? MonetaryUnitId { get; set; }

    public int ProductionBudget { get; set; }
    public int OriginCountryId { get; set; }
    public List<int> FilmingCountryIds { get; set; } = new();
    public string Language { get; set; }
    public string ShottingFormat { get; set; }
    public string AspectRatio { get; set; }
    public FilmColor FilmColor { get; set; }
    public bool StudentProject { get; set; }
    public bool FirstTimeFilmMaker { get; set; }
    //navigation property
    public int ProjectId { get; set; }

    public AddEditFilmSpecificationCommand()
    {
        SubProjectTypeIds = new List<int>();
        CompletionDate = DateTime.Today;
    }
}

public class AddEditSubProjectTypeFilmSpecificationRequest
{
    public int SubProjectTypeId { get; set; }
    public int FilmSpecificationId { get; set; }
}