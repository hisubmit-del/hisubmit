using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Projects;

namespace HiSubmit.Application.Specifications.Projects
{
    public class ProjectCreditFilterSpecification : HeroSpecification<ProjectCredit>
    {
        public ProjectCreditFilterSpecification(int projectId)
        {
            Criteria = (projectCredit) => projectCredit.ProjectId == projectId;
        }
    }
}
