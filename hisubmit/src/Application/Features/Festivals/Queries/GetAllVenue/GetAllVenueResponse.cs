using System.Collections.Generic;
using HiSubmit.Application.Features.Festivals.Commands.AddEditShowHall;
using HiSubmit.Application.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllVenue
{
    public class GetAllVenueResponse
    {
        public int Id { set; get; }
        public string Name { get; set; }
        public  VenueType VenueType { get; set; }
        public  int ShowHallCount { get; set; }
        public AddEditAddressCommand Address { get; set; }
        public  List<ShowHallDto> ShowHalls { get; set; }
    }

    public class ShowHallDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Capacity { get; set; }
        public int AvailableCapacity { get; set; }

        public int VenueId { get; set; }

        public List<ShowTimeDto> ShowTimes { get; set; }
    }
}
