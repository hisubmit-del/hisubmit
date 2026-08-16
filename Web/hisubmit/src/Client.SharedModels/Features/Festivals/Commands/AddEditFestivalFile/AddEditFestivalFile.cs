using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalFile;

public class AddEditFestivalFileCommand
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FileURL { get; set; }
    public string Description { get; set; }
    public FileFormat FileFormat { get; set; }
    public int FestivalId { get; set; }
    public UploadRequest UploadFileRequest { get; set; }
}