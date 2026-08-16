using System;
using HiSubmit.Application.Specifications.Base;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Specifications.Tickets;

// public sealed class GetAllFestivalTicketSpecification : HeroSpecification<Ticket>
// {
//     public GetAllFestivalTicketSpecification(int festivalId)
//     {
//         Criteria = ticket => ticket.Venue.ProductFestivalId == festivalId;
//     }
// }

public sealed class GetAllTicketFilterSpecification : HeroSpecification<Ticket>
{
    public GetAllTicketFilterSpecification(bool? getActiveTicket,int? festivalId,bool? isEnable, TicketType? ticketType = null)
    {
        AddInclude(p => p.Venue);

        Criteria = ticket =>
                (isEnable==null || ticket.IsEnable==isEnable.Value)&&
                (festivalId==null || festivalId.Value==0 || (ticket.Venue != null && ticket.Venue.FestivalId==festivalId.Value))&&
                (getActiveTicket == null
                 || (getActiveTicket.Value && DateTime.Now > ticket.OpenDate && DateTime.Now < ticket.CloseDate)
                 || (!getActiveTicket.Value && ! (DateTime.Now > ticket.OpenDate && DateTime.Now < ticket.CloseDate)))
                && (ticketType == null || ticket.TicketType == ticketType)
            ;
    }
}
