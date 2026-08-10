using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.SpecialAccounts.Queries;

public class GetUserAccountTypeQuery 
{
    public string UserId { get; set; }
}


public class GetUserAccountTypeResponse
{
    public int Id { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public FeeStatus FeeStatus { get; set; }
}