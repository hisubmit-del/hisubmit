using System;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums.Festivals;

namespace HiSubmit.Domain.Entities.Payments;

public class FestivalPaymentItem:AuditableEntity<int>
{
    public  double Amount { get; set; }
    public  int FestivalId { get; set; }
    public DateTime  PaidDate { get; set; }
    public  Festival Festival { get; set; }
    public  string TrackNumber { get; set; }
    public  FestivalPaymentType Type { get; set; }
}
