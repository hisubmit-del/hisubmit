#nullable enable
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.SubProjectTypes.Queries.GetAll;

public class GetAllSubProjectTypeQuery 
{
    public ProjectType? ProjectType { get; set; }
    public string? SubIdString { get; set; }
}