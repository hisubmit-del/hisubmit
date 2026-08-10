using Hisubmit.Client.SharedModels.Enums.Festivals;

namespace  Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Commands.AddEdit;

public class AddEditFestivalPaymentInformationCommand
{
    public  int Id { get; set; }
    public FestivalPaymentType Type { get; set; }
    public string PaypalEmail { get; set; }
    public string CardNumber { get; set; }
    public string CVC { get; set; }
    public string Expires { get; set; }
    
    public  int FestivalId { get; set; }
}

