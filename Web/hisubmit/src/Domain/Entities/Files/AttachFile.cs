using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Advertise;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Files;

public class AttachFile:AuditableEntity<int>
{
    public string Title { get; set; }
    public string Url { get; set; }
    public FileFormat FileFormat { get; set; }

    public int AdvertiseRequestId { get; set; }
    public AdvertiseRequest AdvertiseRequest { get; set; }
}
