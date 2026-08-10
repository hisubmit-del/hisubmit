using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Projects;

namespace HiSubmit.Application.Specifications.Projects
{
    public class ScreenAwardSpecification : HeroSpecification<ScreeningAward>
    {
        public ScreenAwardSpecification(int projectId)
        {
            Criteria = (p) => p.ProjectId == projectId;
        }
    }
}



