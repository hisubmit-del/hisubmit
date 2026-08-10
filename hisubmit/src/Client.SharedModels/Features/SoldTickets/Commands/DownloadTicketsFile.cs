namespace Hisubmit.Client.SharedModels.Features.SoldTickets.Commands;

public class DownloadTicketsFileQuery 
{
    public int SoldTicketId { get; set; }
}



public class DownloadTicketFileResponse
{
    public string MimeType { get; set; } 
    public string FileName { get; set; }
    public byte[] File { get; set; }
}