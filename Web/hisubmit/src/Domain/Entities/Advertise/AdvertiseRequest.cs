using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Files;
using HiSubmit.Domain.Enums.Advertises;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Advertise;

public class AdvertiseRequest:AuditableEntity<int>
{
    public string Text { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }
    public List<Image> Images { get; set; }
    public string Description { get; set; }
    public List<AttachFile> Files { get; set; }
    public AdvertiseType AdvertiseType { get; set; }
}
