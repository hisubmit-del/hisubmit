using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;

public class GetFilmSpecificationDetailRequest 
{
    public int ProjectId { get; set; }
}

 
public class GetFilmSpecificationDetailResponse
{
    public int Id { get; set; }
    public List<int> SubProjectTypeIds { get; set; }
    public string Genre { get; set; }
    public int RunTimeHours { get; set; }
    public int RunTimeMinutes { get; set; }
    public int RunTimeSecounds { get; set; }
    public DateTime CompletionDate { get; set; }
    public MonetaryUnitDto MonetaryUnit { get; set; }
    public int ProductionBudget { get; set; }
    public int OriginCountryId { get; set; }
    public  string OriginCountryName { get; set; }
        
    public List<int> FilmingCountryIds { get; set; }
    public  List<string> FilmingCountriesName { get; set; }
    public string Language { get; set; }
    public string ShottingFormat { get; set; }
    public string AspectRatio { get; set; }
    public FilmColor FilmColor { get; set; }
    public bool StudentProject { get; set; }
    public bool FirstTimeFilmMaker { get; set; }
    //navigation property
    public int ProjectId { get; set; }

    public GetFilmSpecificationDetailResponse()
    {
        CompletionDate = DateTime.Today;
    }
}

public class MonetaryUnitDto
{
    public int Id { get; set; }
    public string  Name { get; set; }
}