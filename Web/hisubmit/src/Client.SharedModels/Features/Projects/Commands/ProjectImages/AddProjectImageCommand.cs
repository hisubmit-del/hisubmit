using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;


namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.ProjectImages;

public class AddProjectImageCommand 
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string State { get; set; }
    public int ProjectId { get; set; }

    public UploadRequest UploadRequest { get; set; }

    public AddProjectImageCommand()
    {
        UploadRequest = new UploadRequest
        {
            UploadType = UploadType.ProjectFile
        };
    }
}

