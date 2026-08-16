using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditShowHall;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllShowHall;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetVenueById
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
