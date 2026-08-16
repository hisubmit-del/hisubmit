using System.Linq;
using AutoMapper;
using HiSubmit.Application.Features.SoldTickets.Commands;
using HiSubmit.Application.Features.SoldTickets.Queries;
using HiSubmit.Application.Features.Tickets.Commands.AddEditTickets;
using HiSubmit.Application.Features.Tickets.Queries.GetAllTicket;
using HiSubmit.Application.Features.Tickets.Queries.GetTicketById;
using Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetTicketById;
using HiSubmit.Domain.Entities.Festivals.Tickets;

namespace HiSubmit.Application.Mappings;

public class TicketProfile:Profile
{
    public TicketProfile()
    {
        CreateMap<Ticket, AddEditTicketsCommand>().ReverseMap()
            .ForMember(des=>des.ShowTimeTickets,
                map=>map.Ignore());

        CreateMap<Ticket, GetAllTicketResponse>()
            .ForMember(des => des.VenueName, 
                map => map.MapFrom(src => src.Venue == null ? null : src.Venue.Name))
            .ReverseMap();

        CreateMap<Ticket, GetTicketByIdResponse>()
            .ForMember(p => p.ShowTimesId, 
                map => map.MapFrom(src => src.ShowTimeTickets.Select(p=>p.ShowTimeId).ToHashSet()));

        CreateMap<SoldTicket, AddSoldBadgeCommand>().ReverseMap();
        CreateMap<SoldTicket, AddSoldTicketCommand>().ReverseMap();

        CreateMap<SoldTicket, GetAllSoldTicketResponse>()
            .ForMember(des => des.TicketTitle, 
                map => map.MapFrom(src => src.Ticket.Title))
            .ForMember(des => des.TicketType, 
                map => map.MapFrom(src => src.Ticket.TicketType));
    }
}
