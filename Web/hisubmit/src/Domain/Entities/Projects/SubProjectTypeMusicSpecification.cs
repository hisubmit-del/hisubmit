using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Projects
{
    public class SubProjectTypeMusicSpecification : AuditableEntity<int>
    {
        public int SubProjectTypeId { get; set; }
        public int MusicSpecificationId { get; set; }

        public MusicSpecification MusicSpecification { get; set; }
        public SubProjectType SubProjectType { get; set; }
    }
}
