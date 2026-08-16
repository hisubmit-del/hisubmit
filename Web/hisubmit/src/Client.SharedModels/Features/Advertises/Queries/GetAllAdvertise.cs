using Hisubmit.Client.SharedModels.Wrapper;
using Hisubmit.Client.SharedModels.Enums.Advertises;

namespace Hisubmit.Client.SharedModels.Features.Advertises.Queries;

public class GetAllAdvertiseRequest:
    PagedRequest
{
    public string  SearchString { get; set; } 
}


public class GetAllAdvertiseResponse
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string Email { get; set; }
    public string Text { get; set; }
    public string UserId { get; set; }
    public  string UserName { get; set; }
    public  DateTime CreatedOn { get; set; }
    public AdvertiseType AdvertiseType { get; set; }
}