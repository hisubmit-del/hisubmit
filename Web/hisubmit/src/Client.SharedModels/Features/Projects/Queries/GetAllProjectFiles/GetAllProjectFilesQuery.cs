using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UploadProjectFile;

namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles
{
    public class GetAllProjectFilesQuery 
    {
        public int ProjectId { get; set; }
    }

    public class GetAllProjectFileResponse
    {
        public int Id { get; set; }
        public string FileURL { get; set; }
        public bool IsLocalFile { get; set; }
        public string LocalFileURL { get; set; }
        public string Password { get; set; }

        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string FileDescription { get; set; }
        public FileFormat? FileFormat { get; set; }

        public int Order { get; set; }

        public bool IsMainFile { get; set; }
        public ProjectFilePosition Position { get; set; }
    }

  
}