using System;
using System.Collections.Generic;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class Venue : AuditableEntity<int>
    {
        public string Name { get; set; }

        public int FestivalId { get; set; }
        public Festival Festival { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        
        public  VenueType VenueType { get; set; }
        
        public  List<ShowHall> ShowHalls { get; set; }
        public  int Capacity { get; set; }
        public  int AvailableCapacity { get; set; }
        public List<Ticket> Tickets { get; set; }
    }

    public class ShowHall:AuditableEntity<int>
    {
        public  string Title { get; set; }
        public  int Capacity { get; set; }
        public  int AvailableCapacity { get; set; }
        
        public  Venue Venue { get; set; }
        public  int VenueId { get; set; }
        
        public  List<ShowTime> ShowTimes { get; set; }
    }


    public class ShowTime:AuditableEntity<int>
    {
        public  DateTime OpenDate { get; set; }
        public  DateTime CloseDate { get; set; }
        
        public  int AvailableCapacity { get; set; }
        public  string Name { get; set; }
        public  int ShowHallId { get; set; }
        public  ShowHall ShowHall { get; set; }
    }
}
