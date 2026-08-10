using HiSubmit.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Domain.Entities.Payments;

public class Cart : AuditableEntity<int>
{
    public bool Paid { get; set; }
    public decimal Price { get; set; }
    public string UserId { get; set; }
    public  string OrderId { get; set; }
    
    public string PaymentId { get; set; }
    public string PayerId { get; set; }
    public string Email { get; set; }
    public DateTime CartDate { get; set; }
    public List<CarTItem> CartItems { get; set; }
}
