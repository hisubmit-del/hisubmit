using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEdiitEventOrginizer;

public class AddEditEventOrginizerCommand
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public int FestivalId { get; set; }
    public string ImageName { get; set; }
    public UploadRequest Image { get; set; } = new();
}