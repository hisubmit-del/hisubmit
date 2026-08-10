using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Advertise;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Festivals;

public class Image:AuditableEntity<int>
{
    public string Title { get; set; }
    public  string Url { get; set; }
    public  ImageType ImageType { get; set; }
    public  int? FestivalId { get; set; }
    public  Festival Festival { get; set; }

    public int? AdvertiseRequestId { get; set; }
    public AdvertiseRequest AdvertiseRequest { get; set; }
}
