using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Projects
{
    public class ProjectFile : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string FileURL { get; set; }
        public bool IsLocalFile { get; set; }
        public bool IsMainFile { get; set; }
        public string LocalFileURL { get; set; }
        public string Password { get; set; }

        public FileFormat? FileFormat { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        
        public int Order { get; set; }

        public string FileDescription { get; set; }

        public ProjectFilePosition Position { get; set; }
    }



    public enum ProjectFilePosition:short
    {
        Header=0,
        SideBarFile=1,
        Gallery=2
    }
}
