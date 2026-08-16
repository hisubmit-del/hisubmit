using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Judgings.Queries.Detail;

public class GetJudgingDetailQuery
{
    public int FestivalId { get; set; }
    public  ProjectType ProjectType { get; set; }
}