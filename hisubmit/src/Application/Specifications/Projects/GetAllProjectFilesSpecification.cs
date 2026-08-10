using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Projects;

namespace HiSubmit.Application.Specifications.Projects
{
    public class GetAllProjectFilesSpecification : HeroSpecification<ProjectFile>
    {
        public GetAllProjectFilesSpecification(int projectId)
        {
            Criteria = (projectFiles) => projectFiles.ProjectId == projectId;
        }
    }
}
