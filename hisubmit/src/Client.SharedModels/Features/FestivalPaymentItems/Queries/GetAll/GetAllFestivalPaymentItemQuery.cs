using Hisubmit.Client.SharedModels.Features.Wrapper;
using Hisubmit.Client.SharedModels.Wrapper;


namespace Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Queries.GetAll;

public class GetAllFestivalPaymentItemQuery
    :PagedRequest
{
    public  string SearchString { get; set; }
    public  int? FestivalId { get; set; }
    public  RequestAccountType AccountType { get; set; }
}

