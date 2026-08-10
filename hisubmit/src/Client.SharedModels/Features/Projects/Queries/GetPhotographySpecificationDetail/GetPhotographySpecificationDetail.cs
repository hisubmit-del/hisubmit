namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetMusicSpecificationDetail;

public class GetPhotographySpecificationDetailQuery
{
    public int ProjectId { get; set; }
}

public class GetPhotographySpecificationDetailResponse
{
    public int Id { get; set; }

    public string Genre { get; set; }
    public DateTime TakenDate { get; set; }
    public int OriginCountryId { get; set; }
    public string Camera { get; set; }
    public string Lens { get; set; }
    public string FocalLength { get; set; }
    public string ShutterSpeed { get; set; }
    public string Aperture { get; set; }
    public string Iso_Film { get; set; }
    public bool StudentProject { get; set; }

    //navigation propety
    public int ProjectId { get; set; }

    public List<int> SubProjectTypeIds { get; set; }
    public string OriginCountryName { get; set; }

    public GetPhotographySpecificationDetailResponse()
    {
        TakenDate = DateTime.Today;
    }
}