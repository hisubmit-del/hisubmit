using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Domain.Enums;
using System;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Projects
{
    public class FilmSpecification : AuditableEntity<int>
    {
        public List<SubProjectTypeFilmSpecification> ProjectTypes { get; set; }
        public string Genre { get; set; }
        public int RunTimeHours { get; set; }
        public int RunTimeMinutes { get; set; }
        public int RunTimeSecounds { get; set; }
        public DateTime CompletionDate { get; set; }
        public MonetaryUnit MonetaryUnit { get; set; }
        public int? MonetaryUnitId { get; set; }
        public int ProductionBudget { get; set; }
        public int OriginCountryId { get; set; }
        public Country OriginCountry { get; set; }

        // public int FilmingCountryId { get; set; }
        // public Country FilmingCountry { get; set; }
        
        public List<Country> FilmingCountries { get; set; }


        public string Language { get; set; }
        public string ShottingFormat { get; set; }
        public string AspectRatio { get; set; }
        public FilmColor FilmColor { get; set; }
        public bool StudentProject { get; set; }
        public bool FirstTimeFilmMaker { get; set; }


        //navigation property
        public int ProjectId { get; set; }
        public Project Project { get; set; }


        public FilmSpecification()
        {
            ProjectTypes = new List<SubProjectTypeFilmSpecification>();
        }
    }
}
