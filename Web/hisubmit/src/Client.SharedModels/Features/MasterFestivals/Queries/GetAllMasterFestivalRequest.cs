using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalNames;
using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Hisubmit.Client.SharedModels.Features.MasterFestivals.Queries;

public class GetAllMasterFestivalRequest:PagedRequest
{
    
}

public class GetAllMasterFestivalResponse
{
    public string Name { get; set; }
    public int ActivePeriod { get; set; }
    public int Id { get; set; }

    public List<GetFestivalNamesResponse> Festivals { get; set; } = new();
}