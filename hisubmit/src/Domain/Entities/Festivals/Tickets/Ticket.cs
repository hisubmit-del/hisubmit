using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;
using System;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Festivals.Tickets;

public class Ticket : AuditableEntity<int>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime CloseDate { get; set; }
    public bool AddManagerPercentage { get; set; }
    public int Cost { get; set; }

    public List<SubmissionQuestion> SubmissionQuestions { get; set; }

    public int? VenueId { get; set; }
    public Venue Venue { get; set; }

    //EventDate
    public  DateTime EventDate { get; set; }
    //Type
    public  TicketType TicketType { get; set; }
    public  bool IsEnable { get; set; }
    //Capacity
    public  int Capacity { get; set; }
    public  int AvailableCapacity { get; set; }

    //وضعیت نمایش بلیط 
    public ShowInSiteStatus Status { get; set; }

    public List<ShowHallTicket> ShowHallTickets { get; set; }
    public List<ShowTimeTicket> ShowTimeTickets { get; set; }
}

public class ShowHallTicket : AuditableEntity<int>
{
    public int TicketId { get; set; }
    public int ShowHallId { get; set; }
    public Ticket Ticket { get; set; }
    public ShowHall ShowHall { get; set; }
}

public class ShowTimeTicket : AuditableEntity<int>
{
    public int TicketId { get; set; }
    public int ShowTimeId { get; set; }
    public  string Name { get; set; }
    public Ticket Ticket { get; set; }
    public ShowTime ShowTime { get; set; }
}
