using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalVenue;

public class AddEditFestivalVenueCommand 
{
    public int FestivalId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public VenueType VenueType { get; set; }
    public AddEditAddressCommand Address { get; set; }

    public AddEditFestivalVenueCommand()
    {
        Address = new AddEditAddressCommand();
    }
}