using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Projects
{
    public class ProjectItemPerson : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PriorCredit { get; set; }

        public int ProjectCreditId { get; set; }
        public ProjectCredit ProjectCredit { get; set; }
    }
}
