using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Projects
{
    public class MonetaryUnit : AuditableEntity<int>
    {
        public string Name { get; set; }
    }
}
