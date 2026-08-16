using AutoMapper;
using Hisubmit.Client.SharedModels.Features.Tickets.Commands.AddEditTickets;
using Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetTicketById;

namespace HiSubmit.Client.Infrastructure.Mappings;

public class TicketProfile:Profile
{
    public TicketProfile()
    {
        CreateMap<AddEditTicketsCommand, GetTicketByIdResponse>()
            .ReverseMap();
    }
}
