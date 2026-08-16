using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Locations;
using System;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Projects
{
    public class XrVrSpecification : AuditableEntity<int>
    {
        public List<SubProjectTypeVRXrSpecification> ProjectType { get; set; }
        public string Genre { get; set; }
        public int RunTimeHours { get; set; }
        public int RunTimeMinutes { get; set; }
        public int RunTimeSecounds { get; set; }
        public bool VariableRunTime { get; set; }
        public string DescriptionRunTime { get; set; }
        public int MinRunTimeHours { get; set; }
        public int MinRunTimeMinutes { get; set; }
        public int MinRunTimeSecounds { get; set; }
        public int MaxTimeHours { get; set; }
        public int MaxTimeMinutes { get; set; }
        public int MaxTimeSecounds { get; set; }
        public int AvgTimeHours { get; set; }
        public int AvgTimeMinutes { get; set; }
        public int AvgTimeSecounds { get; set; }

        public DateTime CompletionDate { get; set; }
        public MonetaryUnit MonetaryUnit { get; set; }
        public int? MonetaryUnitId { get; set; }
        public int ProductionBudget { get; set; }
        public int OriginCountryId { get; set; }
        public Country OriginCountry { get; set; }

        public string Language { get; set; }
        public bool StudentProject { get; set; }


        //navigationProperty
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public XrVrSpecification()
        {
            ProjectType = new List<SubProjectTypeVRXrSpecification>();
        }
    }
}
