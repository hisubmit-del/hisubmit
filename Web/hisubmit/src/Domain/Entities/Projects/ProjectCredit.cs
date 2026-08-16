using HiSubmit.Domain.Contracts;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Projects
{
    public class ProjectCredit:AuditableEntity<int>
    {
        public string Title { get; set; }
        public List<ProjectItemPerson> ProjectItemPeople { get; set; }

        public Project Project { get; set; }
        public int ProjectId { get; set; }
        public string ImageUrl { get; set; }
    }
}
