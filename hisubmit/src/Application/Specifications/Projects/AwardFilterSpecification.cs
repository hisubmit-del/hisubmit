using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Projects;

namespace HiSubmit.Application.Specifications.Projects
{
    public class AwardFilterSpecification : HeroSpecification<Award>
    {
        public AwardFilterSpecification(int projectId)
        {
            Criteria=(p)=>p.ProjectId==projectId;
        }
    }
}
