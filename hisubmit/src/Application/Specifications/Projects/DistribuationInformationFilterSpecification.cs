using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Projects;

namespace HiSubmit.Application.Specifications.Projects
{
    public class DistribuationInformationFilterSpecification : HeroSpecification<DistributionInformation>
    {
        public DistribuationInformationFilterSpecification(int projectId)
        {
            Criteria = (p) => p.ProjectId == projectId;
        }
    }
}
