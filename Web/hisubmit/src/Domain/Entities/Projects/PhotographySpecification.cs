using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Locations;
using System;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Projects
{
    public class PhotographySpecification : AuditableEntity<int>
    {
        public List<PhotographySpecificationSubProjectType> PhotographySpecificationSubProjectTypes { get; set; }
        public string Genre { get; set; }
        public DateTime TakenDate { get; set; }
        public int OriginCountryId { get; set; }
        public Country OriginCountry { get; set; }
        public string Camera { get; set; }
        public string Lens { get; set; }
        public string FocalLength { get; set; }
        public string Location { get; set; }
        public string ShutterSpeed { get; set; }
        public string Aperture { get; set; }
        public string Iso_Film { get; set; }
        public bool StudentProject { get; set; }

        //navigation propety
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public PhotographySpecification()
        {
            PhotographySpecificationSubProjectTypes = new List<PhotographySpecificationSubProjectType>();
        }
    }


    public class PhotographySpecificationSubProjectType :AuditableEntity<int>
    {
        public int SubProjectTypeId { get; set; }

        public int PhotographySpecificationId { get; set; }
        public SubProjectType SubProjectType { get; set; }
        public PhotographySpecification PhotographySpecification { get; set; }
    }
}
