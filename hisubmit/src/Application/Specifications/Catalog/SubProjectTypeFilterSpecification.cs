using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Application.Specifications.Catalog
{
    public class SubProjectTypeFilterSpecification:HeroSpecification<SubProjectType>
    {
        public SubProjectTypeFilterSpecification(ProjectType? projectType,IReadOnlyCollection<int> ids)
        {
            Criteria = subProjectType => (projectType == null || subProjectType.ProjectType == projectType)&&
                                           ( ids.Any(id=>subProjectType.Id==id));
        }
    }
}
