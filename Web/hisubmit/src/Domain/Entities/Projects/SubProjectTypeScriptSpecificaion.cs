using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Projects
{
    public class SubProjectTypeScriptSpecificaion : AuditableEntity<int>
    {
        public int SubProjectTypeId { get; set; }

        public int ScriptSpecificationId { get; set; }
        public SubProjectType  SubProjectType { get; set; }
        public ScriptSpecification ScriptSpecification { get; set; }

    }
}
