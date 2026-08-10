using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiSubmit.Domain.Entities.Catalog;

namespace HiSubmit.Domain.Entities.Locations
{
    public class Address:AuditableEntity<int>
    {
        public string Text { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }



        public int? FestivalId { get; set; }
        public Festival Festival { get; set; }

        public int? SubmissionFestivalId { get; set; }
        public Festival SubmissionFestival { get; set; }

        public int? VenueId { get; set; }
        public Venue Venue { get; set; }

        public int? ProjectId { get; set; }
        public Project Project { get; set; }
        
        public  int? ProductId { get; set; }
        public  Product Product { get; set; }
        
        public  string MapLocation { get; set; }
    }
}
