using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalNames;

public class GetFestivalNamesQuery :PagedRequest
{
       
    public  string FestivalIdString { get; set; }
    
    public List<int> GetFestivalId()
    {
        return FestivalIdString.Split(',').Select(int.Parse).ToList();
    }
}


public class GetFestivalNamesResponse
{
    public  int Id { get; set; }
    public  string Name { get; set; }
    public  string LogoURL { get; set; }
    public  string Email { get; set; }
    public int YearsRunning { get; set; }
    public int? FestivalMasterId { get; set; }

    public bool AdminLogin { get; set; } 
}
