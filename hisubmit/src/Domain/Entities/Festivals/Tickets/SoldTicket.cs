using System;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Festivals.Tickets;

public class SoldTicket:AuditableEntity<int>
{
    public decimal Cost { get; set; }
    //public int Income { get; set; }
    public decimal ShareFestivalIncome { get; set; }
    public  int Count { get; set; }
    public  Guid SerialNumber { get; set; }
    public  DateTime BuyDate { get; set; }
    public  string UserId { get; set; }
    public  byte[] QrCode { get; set; }
    public  byte[] PdfFile { get; set; }
    public  int TicketId { get; set; }
    public Ticket Ticket { get; set; }
    
    public  int? ShowTimeId { get; set; }
    public  ShowTime ShowTime { get; set; }
    
    public  bool ForOtherUser { get; set; }
    public  string OtherUserEmail { get; set; }
    
    public  int? ChairNumber { get; set; }
    public  SoldTicketStatus SoldTicketStatus { get; set; }
}