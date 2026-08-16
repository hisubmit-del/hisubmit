using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditAward;

public class UpdateAwardRequest 
{
    public int ProjectId { get; set; }
    public List<AddEditAwardRequest> Awards { get; set; }
}

public class AddEditAwardRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string AwardsWon { get; set; }
    public DateTime? Date { get; set; }

    public int ProjectId { get; set; }
    public string ImageUrl { get; set; }

    public UploadRequest UploadRequest { get; set; } = new();
}