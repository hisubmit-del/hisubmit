using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums.Festivals;

namespace HiSubmit.Domain.Entities.Payments;

public class FestivalPaymentInformation : AuditableEntity<int>
{
    public FestivalPaymentType Type { get; set; }
    public string PaypalEmail { get; set; }
    public string CardNumber { get; set; }
    public string CVC { get; set; }
    public string Expires { get; set; }
    
    public  int FestivalId { get; set; }
    public  Festival Festival { get; set; }
}