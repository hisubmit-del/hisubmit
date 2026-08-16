using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Projects
{
    public class SubProjectType : AuditableEntity<int>
    {
        public ProjectType ProjectType { get; set; }
        public string Name { get; set; }
    }
}
