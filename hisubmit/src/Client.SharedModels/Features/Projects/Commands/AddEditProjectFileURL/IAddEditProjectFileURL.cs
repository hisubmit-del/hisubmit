using System.ComponentModel;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UploadProjectFile;

namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectFileURL;

public class AddEditProjectFileURLRequest 
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public bool IsLocalFile { get; set; }
    public string FileURl { get; set; }
    public string LocalFileURL { get; set; }
    public string Password { get; set; }
    public string FileDescription { get; set; }
    public string Name { get; set; }
    public FileFormat FileFormat { get; set; }
    public bool IsMainFile { get; set; }

    public ProjectFilePosition Position { get; set; }

    public ConflictWays ConflictWays { get; set; }
}


public class AddEditFileUrlResponse
{
    public int  FileId { get; set; }
    public bool  HasConflictFile { get; set; }
}


public enum ConflictWays
{
    [Description("Cancel saving ")]
    Default=0,
    [Description("Delete other header files")]
    DeleteFiles=1,
    [Description("Move all other files to sidebar files")]
    MoveToFiles=2,
    [Description("Move all other files to Gallery(only images)")]
    MoveToGallery=3
}