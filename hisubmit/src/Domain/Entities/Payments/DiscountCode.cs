using System;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;

namespace HiSubmit.Domain.Entities.Payments;

public class DiscountCode:AuditableEntity<int>
{
    public int?  FestivalId { get; set; }
    public Festival Festival { get; set; }
    public string CartItemTypes { get; set; }
    public DateTime? ExpiredTime { get; set; }
    public short? Count { get; set; }
    public DiscountValueType DiscountValueType { get; set; }
    public double DiscountValue { get; set; }

    public bool Enable { get; set; }
    public string Description { get; set; }
    public string Code { get; set; }
}