using System.Collections.Generic;
using HiSubmit.Application.Features.Festivals.Commands.AddEditShowHall;
using HiSubmit.Application.Features.Festivals.Queries.GetAllShowHall;
using HiSubmit.Application.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.Festivals.Queries.GetVenueById
{
    public class GetVenueByIdResponse
    {
        public int Id { get; set; }
        public int FestivalId { get; set; }
        public string Name { get; set; }
        public AddEditAddressCommand Address { get; set; }
        public  VenueType VenueType { get; set; }
        public  List<GetAllShowHallResponse> ShowHalls { get; set; }

        public GetVenueByIdResponse()
        {
            ShowHalls = new List<GetAllShowHallResponse>();
        }
    }
}
