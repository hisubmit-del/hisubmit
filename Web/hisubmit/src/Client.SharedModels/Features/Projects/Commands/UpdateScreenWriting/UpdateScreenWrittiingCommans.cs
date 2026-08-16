using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.UpdateScreenWritings;

public class UpdateScreenWritingRequest
{
    public int ProjectId { get; set; }
    public List<AddEditScreenWritingRequest> ScreenWritings { get; set; }

    public UpdateScreenWritingRequest()
    {
        ScreenWritings = new List<AddEditScreenWritingRequest>();
    }
}

    

public class AddEditScreenWritingRequest
{
    public int Id { get; set; }
    public string City { get; set; }
    public string Title { get; set; }
    public int CountryId { get; set; }
    public int ProjectId { get; set; }
    public string Premiere { get; set; }
    public string ImageUrl { get; set; }
    public string AwardSelection { get; set; }

    public UploadRequest UploadRequest { get; set; } = new();
    public DateTime? ScreeningDate { get; set; }
}