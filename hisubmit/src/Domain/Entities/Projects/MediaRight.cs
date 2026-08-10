using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Projects
{
    public class MediaRight:AuditableEntity<int>
    {
        public string Name { get; set; }
    }
}
