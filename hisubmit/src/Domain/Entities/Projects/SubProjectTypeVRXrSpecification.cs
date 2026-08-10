using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Projects
{
    public class SubProjectTypeVRXrSpecification : AuditableEntity<int>
    {
        public int SubProjectTypeId { get; set; }
        public int XrVrSpecificationId { get; set; }

        public XrVrSpecification XrVrSpecification { get; set; }
        public SubProjectType SubProjectType { get; set; }
    }
}
