using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Locations;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Projects
{
    public class ScriptSpecification : AuditableEntity<int>
    {
        public List<SubProjectTypeScriptSpecificaion> ProjectTypes { get; set; }
        public string Genre { get; set; }
        public int NumberOfPage { get; set; }
        public int OriginCountryId { get; set; }
        public Country OriginCountry { get; set; }
        public string Language { get; set; }
        public bool StudentProject { get; set; }
        public bool FirstTimeScreenWrite { get; set; }
         
        //navigation property
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public ScriptSpecification()
        {
            ProjectTypes = new List<SubProjectTypeScriptSpecificaion>();
        }
    }
}
