using System;

namespace HiSubmit.Application.Models.Tickets;

public class TicketViewModel
{
    public  string TicketTitle { get; set; }
    public  byte[] QRCode { get; set; }
    public  string ShowTimeName { get; set; }
    public  DateTime StartDate { get; set; }
    public  DateTime EndDate { get; set; }
    public  string Email { get; set; }
    public  string GuidString { get; set; }
    public  int Count { get; set; }
}
