using System;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Advertise;

public class AdvertiseBanner:AuditableEntity
{
    public  string Url { get; set; }
    public  string Title { get; set; }
    public  DateTime OpenDateTime { get; set; }
    public  DateTime CloseDateTime { get; set; }
    public  AdvertiseBannerPosition Position { get; set; }
}
