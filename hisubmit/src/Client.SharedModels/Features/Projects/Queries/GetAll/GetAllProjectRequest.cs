using Hisubmit.Client.SharedModels.Wrapper;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAll
{
    public class GetAllProjectRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public string UserId { get; set; }
        
        public string Title { get; set; }
        public ProjectType? ProjectType { get; set; }
        
        public bool? StudentProject { get; set; }
        public bool GetCurrentUserProjects { get; set; }
    }

    public class GetAllProjectResponse
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string FileURl { get; set; }

        public bool StudentProject { get; set; }
        public DateTime CreatedOn { get; set; } 
        public ProjectType ProjectType { get; set; }

    }

}
