using System;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Payments;

public class UserSpecialPeriod:AuditableEntity<int>
{
     public  decimal Cost { get; set; }
     public  string UserId { get; set; }
     public  DateTime OpenDateTime { get; set; }
     public  StatusFeePeriod Period { get; set; }
     public  DateTime CloseDateTime { get; set; }
     public  UserSpecialAccountStatus Status { get; set; }
}