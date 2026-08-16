using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Locations;
using System;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Projects
{
    public class MusicSpecification : AuditableEntity<int>
    {
        public List<SubProjectTypeMusicSpecification> ProjectType { get; set; }

        public string Genre { get; set; }
        public int RunTimeHours { get; set; }
        public int RunTimeMinutes { get; set; }
        public int RunTimeSecounds { get; set; }
        public DateTime CompletionDate { get; set; }
        public int OriginCountryId { get; set; }
        public Country OriginCountry { get; set; }

        public string Language { get; set; }

        public bool StudentProject { get; set; }

        //navigation Property
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public MusicSpecification()
        {
            ProjectType = new List<SubProjectTypeMusicSpecification>();
        }
    }

    public class ArtSpecification:AuditableEntity<int>
    {

    }
}
