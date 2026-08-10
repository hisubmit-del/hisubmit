using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Projects.Queries.GetAll;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Specifications.Projects;

public  class ProjectsFilterSpecification:HeroSpecification<Project>
{
    public ProjectsFilterSpecification(GetAllProjectQuery query)
    {
        Criteria = (p) => p.UserId == query.UserId
                          && (query.ProjectType==null || p.ProjectType==(ProjectType)query.ProjectType)
                          && (query.StudentProject==null || p.StudentProject==query.StudentProject)
                          && (string.IsNullOrWhiteSpace(query.Title) || p.Title.Contains(query.Title))
            ;
    }
}