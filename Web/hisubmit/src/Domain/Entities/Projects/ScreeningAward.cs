using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Locations;
using System;

namespace HiSubmit.Domain.Entities.Projects
{
    public class ScreeningAward:AuditableEntity<int>
    {
        public DateTime ScreeningDate { get; set; }
        public string City { get; set; }
        public Country Country { get; set; }
        public int CountryId { get; set; }
        public string Premiere { get; set; }
        public string AwardSelection { get; set; }
        public string Title { get; set; }
        //navigation property
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        
        public string ImageUrl { get; set; }
    }
}
